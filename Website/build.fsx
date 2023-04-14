open System.IO
open System.Net.Http
open System.Net.Http.Json
open System.Text.Json
open System
open System.Diagnostics
open System.Web

let outputDirectory = "_site"
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

let runProcess filename args =
    let startInfo = ProcessStartInfo(filename)
    addArguments startInfo args
    use p = Process.Start(startInfo)
    p.WaitForExit()
    ensureSuccessExitCode p

Directory.CreateDirectory(outputDirectory)
writeIndexHtml()
copyToOutput "fonts/*.woff2"
copyToOutput "icons/*.svg"
copyToOutput "*.css"
copyToOutput "*.js"

// These links are embedded in Funcky.Analyzers and should keep working
generateRedirect "analyzer-rules/" "/funcky/book/analyzer-rules/"
generateRedirect "analyzer-rules/λ0001.html" "/funcky/book/analyzer-rules/λ0001.html"

runProcess "mdbook" [
    "build";
    Path.Combine(__SOURCE_DIRECTORY__, "..", "Documentation");
    "--dest-dir";
    Path.GetFullPath(Path.Combine(outputDirectory, "book"))]
