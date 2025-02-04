# Analyzers
## Analyzers and Code Fixes In Separate Projects
Analyzers and code fixes are split in two projects.

This is to prevent us from accidentally referencing `Microsoft.CodeAnalysis.*.Workspaces`
which is only available in the IDE, but not during `dotnet build`.

## BuiltinAnalyzers
These are analyzers that in our eyes are *mandatory* (disallow `TryGetValue`),
so we package them directly in the Funcky package to ensure that every user is using them.

## Roslyn Multi-Targeting
In order to support newer C# features (which in turn requires an upgrade of the `Microsoft.CodeAnalysis.*` packages)
we would break compatibility with older .NET SDK / Visual Studio Versions.

To work around this, Roslyn supports shipping different analyzer assemblies
in the NuGet package based on the Roslyn version i.e. ["multi-targeting" but for Roslyn][multi-targeting].

The .NET SDK supports "multi-targeting" analyzers on the Roslyn version.
This means that we ship multiple copies of the analyzer assembly in the following structure:

```
╰─ analyzers
   ╰─ dotnet
      ├─ cs
      │  ├─ Funcky.Analyzers.dll
      │  ╰─ ...
      ╰─ roslyn4.12
         ╰─ cs
            ├─ Funcky.Analyzers.dll
            ╰─ ...
```

## Roslyn Version Compatibility
There are three documents that help figure out what version of Roslyn to target:
* [.NET compiler platform package version reference](https://learn.microsoft.com/en-us/visualstudio/extensibility/roslyn-version-support): \
  Maps Roslyn version to Visual Studio version
* [.NET SDK, MSBuild, and Visual Studio versioning](https://learn.microsoft.com/en-gb/dotnet/core/porting/versioning-sdk-msbuild-vs): \
  Maps .NET SDK version to Visual Studio version
* [The history of C#](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-version-history): \
  Lists C# versions and their release date which helps find the minimum Visual Studio version for a given C# version.


[multi-targeting]: https://github.com/dotnet/sdk/issues/20355
