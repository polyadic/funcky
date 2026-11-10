open System.IO
open System.Net.Http
open System.Net.Http.Json
open System.Text.Json
open System
open System.Diagnostics
open System.Web

let outputDirectory = "_site"
let sitePrefix = "/funcky"

type Book =
    { Label: string
      Source: string
      /// Path below the site root.
      Path: string }

// The first book is the default: it is the target of the analyzer help links.
// The 3.x sources are checked out by the docs workflow; the book is skipped when they are missing.
let books =
    [ { Label = "4.x"; Source = Path.Combine(__SOURCE_DIRECTORY__, "..", "Documentation"); Path = "book" }
      { Label = "3.x"; Source = Path.Combine(__SOURCE_DIRECTORY__, "..", "_documentation", "3.x", "Documentation"); Path = "book/3.x" } ]
    |> List.where (fun book -> Directory.Exists(book.Source))

let versionSwitcherJs = Path.Combine(__SOURCE_DIRECTORY__, "book", "version-switcher.js")
let indexHtml = Path.Combine(__SOURCE_DIRECTORY__, "index.html")
let redirectHtml = Path.Combine(__SOURCE_DIRECTORY__, "redirect.html")

let getLatestPackageVersion package =
    async {
        use httpClient = new HttpClient()
        let! document = httpClient.GetFromJsonAsync<JsonDocument>($"https://api-v2v3search-0.nuget.org/query?q={package}&skip=0&take=1&prerelease=false&semVerLevel=2.0.0") |> Async.AwaitTask
        return (document.RootElement.GetProperty("data")[0]).GetProperty("version").GetString()
    }

let getLatestFunckyVersion() = getLatestPackageVersion "Funcky"

let renderIndexHtml (template: string, packageVersion, year) =
    template
        .Replace("{{PackageVersion}}", packageVersion)
        .Replace("{{Year}}", year.ToString())

let renderRedirect (template: string) destination =
    template
        .Replace("{{Destination}}", HttpUtility.HtmlEncode(destination))

let writeIndexHtml() =
    let version = getLatestFunckyVersion() |> Async.RunSynchronously
    let indexHtmlContents = renderIndexHtml(File.ReadAllText(indexHtml), version, DateTime.Today.Year)
    File.WriteAllText(Path.Combine(outputDirectory, Path.GetFileName(indexHtml)), contents = indexHtmlContents)

let copyFileToOutput source =
    let relativePath = Path.GetRelativePath(relativeTo = __SOURCE_DIRECTORY__, path = source)
    let target = Path.Combine(outputDirectory, relativePath)
    Directory.CreateDirectory(Path.GetDirectoryName(target)) |> ignore
    File.Copy(source, target, overwrite = true)

let isFileInOutputDirectory path =
    let absoluteTargetPath = Path.GetFullPath(outputDirectory) + Path.DirectorySeparatorChar.ToString()
    let absolutePath = Path.GetFullPath(path)
    absolutePath.StartsWith(absoluteTargetPath)

let copyToOutput (pattern) =
    Directory.GetFiles(__SOURCE_DIRECTORY__, searchPattern = pattern, searchOption = SearchOption.AllDirectories)
        |> Seq.where (fun p -> not (isFileInOutputDirectory p))
        |> Seq.iter copyFileToOutput

let generateRedirect (source: string) destination =
    let filePath =
        if source.EndsWith('/') then
            Path.Combine(outputDirectory, source, "index.html")
        else
            Path.Combine(outputDirectory, source)
    Directory.CreateDirectory(Path.GetDirectoryName(filePath)) |> ignore
    File.WriteAllText(filePath, contents = (renderRedirect (File.ReadAllText(redirectHtml)) destination))

let ensureSuccessExitCode (p: Process) =
    if p.ExitCode <> 0 then
        raise (IOException($"The process {p.StartInfo.FileName} exited with a non-zero exit code {p.ExitCode}"))

let addArguments (info: ProcessStartInfo) args =
    args |> Seq.iter info.ArgumentList.Add

let runProcess filename args environment =
    let startInfo = ProcessStartInfo(filename)
    addArguments startInfo args
    environment |> Seq.iter (fun (name: string, value: string) -> startInfo.Environment[name] <- value)
    use p = Process.Start(startInfo)
    p.WaitForExit()
    ensureSuccessExitCode p

Directory.CreateDirectory(outputDirectory)
writeIndexHtml()
copyToOutput "fonts/*.woff2"
copyToOutput "icons/*.svg"
copyToOutput "*.css"
copyToOutput "*.js"

let bookUrl book = $"{sitePrefix}/{book.Path}/"

let buildBook book =
    // The switcher is injected from here, so that the books of older branches get it without a change to their sources.
    File.Copy(versionSwitcherJs, Path.Combine(book.Source, Path.GetFileName(versionSwitcherJs)), overwrite = true)
    runProcess
        "mdbook"
        [ "build"; book.Source; "--dest-dir"; Path.GetFullPath(Path.Combine(outputDirectory, book.Path)) ]
        [ "MDBOOK_OUTPUT__HTML__SITE_URL", bookUrl book
          "MDBOOK_OUTPUT__HTML__ADDITIONAL_JS", JsonSerializer.Serialize([ Path.GetFileName(versionSwitcherJs) ])
          "MDBOOK_OUTPUT__HTML__FOLD__ENABLE", "true"
          "MDBOOK_OUTPUT__HTML__FOLD__LEVEL", "0" ]

let writeVersionsJson book =
    let versions = books |> List.map (fun b -> {| label = b.Label; url = bookUrl b |})
    File.WriteAllText(
        Path.Combine(outputDirectory, book.Path, "versions.json"),
        contents = JsonSerializer.Serialize({| current = book.Label; versions = versions |}))

let analyzerRules book =
    let directory = Path.Combine(book.Source, "src", "analyzer-rules")
    if Directory.Exists(directory) then
        Directory.GetFiles(directory, "λ*.md") |> Seq.map Path.GetFileNameWithoutExtension
    else
        Seq.empty

// These links are embedded in Funcky.Analyzers and should keep working.
// A rule is redirected to the default book, or else to the first book that documents it.
let generateAnalyzerRuleRedirects () =
    books |> List.tryHead |> Option.iter (fun book -> generateRedirect "analyzer-rules/" $"{bookUrl book}analyzer-rules/")
    books
        |> Seq.collect (fun book -> analyzerRules book |> Seq.map (fun rule -> rule, book))
        |> Seq.distinctBy fst
        |> Seq.iter (fun (rule, book) -> generateRedirect $"analyzer-rules/{rule}.html" $"{bookUrl book}analyzer-rules/{rule}.html")

generateAnalyzerRuleRedirects ()

// mdbook clears its destination directory: a book has to be built before the books nested in it.
books |> List.sortBy (fun book -> book.Path.Length) |> List.iter buildBook
books |> List.iter writeVersionsJson
