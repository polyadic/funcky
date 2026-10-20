# Analyzers
## Analyzers and Code Fixes In Separate Projects
Analyzers and code fixes are split in two projects.

This is to prevent us from accidentally referencing `Microsoft.CodeAnalysis.*.Workspaces`
which is only available in the IDE, but not during `dotnet build`.

## Which Package Does a Rule Belong To?
One written rule decides this, so that new rules can be classified without discussion:

* **`Funcky.BuiltinAnalyzers`** (packed into the `Funcky` package, always on, severity *Error*, category `Funcky.Correctness`):
  the flagged code is *outside the supported surface* of the library. Either the member only exists to support a
  compiler feature (`TryGetValue`, the list pattern members), or the value is one the type system permits but Funcky
  does not support (`default(Either<L, R>)`), or the code can never do what it says (a two-element list pattern on an
  option). Nothing is "nicer" here, the code is wrong, so the rule needs no code fix.
* **`Funcky.Analyzers`** (opt-in package, severity *Warning*, category `Funcky.Style`):
  the flagged code works and is supported, but Funcky has a more idiomatic, shorter or cheaper form.
  Every rule in this package comes with a code fix. A rule that is purely a performance hint and may be wrong about
  the cost (λ1015, the eagerly computed fallback) reports an *Info* instead of a warning.

This mirrors how the .NET libraries do it: `System.Text.Json`, `Microsoft.Extensions.*`, `CommunityToolkit.Mvvm` and
EF Core ship their usage diagnostics inside the library package, whereas best-practice hints such as `xunit.analyzers`,
`FluentAssertions.Analyzers` or `Moq.Analyzers` are separate packages.

### Rule IDs
Rule IDs are stable identifiers and never change once shipped, even when a rule moves between the packages
(λ1009 and λ1010 moved to the built-in analyzers in Funcky 3.7.0 and kept their numbers). The number range records
where a rule was born: `λ00xx` built-in, `λ10xx` `Funcky.Analyzers`, `λ11xx` `FunctionalAssert` rules. The rules table in
the documentation is the authority on where a rule lives.

### Built-in Analyzers and Roslyn Versions
The built-in analyzers have to load on every SDK that can consume `Funcky`, so their baseline is Roslyn 3.7
(`Funcky.BuiltinAnalyzers.Roslyn3.7.csproj`). Rules that need newer compiler APIs are guarded with `#if ROSLYN_4_4_0` and
only ship in the `roslyn4.4` asset (`Funcky.BuiltinAnalyzers.csproj`); the `Funcky` package packs both assets with the
same `GetAnalyzerPackFiles` mechanism as `Funcky.Analyzers.Package`, and `Funcky/build/Funcky.targets` removes the
`roslyn4.4` asset on SDKs without analyzer versioning and honours `FunckyDisableBuiltinAnalyzers`.

## Roslyn Multi-Targeting
In order to support newer C# features (which in turn requires an upgrade of the `Microsoft.CodeAnalysis.*` packages)
we would break compatibility with older .NET SDK / Visual Studio Versions.

To work around this, Roslyn supports shipping different analyzer assemblies
in the NuGet package based on the Roslyn version i.e. ["multi-targeting" but for Roslyn][multi-targeting].

The .NET SDK supports "multi-targeting" analyzers on the Roslyn version.
This means that the analyzer assemblies live in a `roslyn<version>` directory, and several such directories
can be shipped side by side:

```
╰─ analyzers
   ╰─ dotnet
      ╰─ roslyn4.4
         ╰─ cs
            ├─ Funcky.Analyzers.dll
            ╰─ ...
```

From the different versions, the highest compatible version is picked. Funcky.Analyzers currently ships a
single asset for Roslyn 4.4 (Visual Studio 2022 17.4, .NET SDK 7.0.100), the oldest version the analyzers
need. Up to Funcky.Analyzers 1.4.2 there was a second, untested `roslyn4.0` asset for Visual Studio 2022
17.0; Funcky.Analyzers 2.0.0 dropped it, as that version is out of support. To add an asset for another Roslyn
version, add a second project file next to `Funcky.Analyzers.csproj` with a different `AnalyzerRoslynVersion`, reference it
from the package project with `PackAsAnalyzer`, and guard the sources that need the newer API with a define.

### Remarks
* SDKs that don't support multi-targeting load analyzers recursively,
  even inside the "versioned" `roslyn*` directories.
    * The `System.Text.Json` package fixes this by shipping a `.targets` file with their packages that
      catches this case by checking for the `SupportsRoslynComponentVersioning` property
      and manually removing discovered analyzers that are incompatible.
    * Our analyzers require at least Roslyn 4.4; SDKs without multi-targeting support (before Roslyn 4.0 / .NET 6.0)
      would load them anyway, so we ship a `.targets` file that removes all our analyzers and warns the user.
    * An SDK that supports multi-targeting but is older than Roslyn 4.4 finds no compatible directory and loads
      nothing, without a warning; that cannot be detected from MSBuild.
* SDKs versions that support multi-targeting also load analyzers
  that are not in a `roslyn*` directory, so to avoid loading analyzers twice
  all analyzers need to be inside a `roslyn*` directory.

## Roslyn Version Compatibility
There are three documents that help figure out what version of Roslyn to target:
* [.NET compiler platform package version reference](https://learn.microsoft.com/en-us/visualstudio/extensibility/roslyn-version-support): \
  Maps Roslyn version to Visual Studio version
* [.NET SDK, MSBuild, and Visual Studio versioning](https://learn.microsoft.com/en-gb/dotnet/core/porting/versioning-sdk-msbuild-vs): \
  Maps .NET SDK version to Visual Studio version
* [The history of C#](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-version-history): \
  Lists C# versions and their release date which helps find the minimum Visual Studio version for a given C# version.


[multi-targeting]: https://github.com/dotnet/sdk/issues/20355
