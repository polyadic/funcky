# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

Funcky is a functional programming library for C#. It encourages idiomatic, side-effect-free
functional C# by leaning on the language's existing monadic interfaces (LINQ, async) rather than
inventing a parallel world. The centerpiece is the `Option` monad; the library also ships
`Result`, `Either`, `Reader`, and `Lazy` monads, a `Unit` type, fundamental functions
(`Identity`, `True`, `False`, …), `IEnumerable` constructors (`Sequence.Return`, `Sequence.Successors`, …),
and a large set of `IEnumerable`/`string`/`IQueryable` extensions.

## Build & test

There is no `.sln`; the solution file is `Funcky.slnx` (XML-based). `dotnet` commands operate on it
automatically from the repo root.

```bash
dotnet restore
dotnet build                       # CI builds with --configuration Release /p:TreatWarningsAsErrors=true
dotnet test                        # runs all test projects
dotnet test Funcky.Test            # one project
dotnet test Funcky.Test --filter "FullyQualifiedName~Pairwise"   # subset by name
dotnet test --framework net10.0    # pin one target framework (tests multi-target too)
```

CI builds on both Linux and Windows with `TreatWarningsAsErrors=true`, so a warning is a build
failure there. The build also runs against two `System.Linq.Async` versions
(`SystemLinqAsyncVersion` env var). When changing behavior that depends on a framework version,
remember tests run on multiple TFMs.

Trimming / AOT compatibility is enforced via `Funcky.TrimmingTest`:

```bash
dotnet publish -c Release Funcky.TrimmingTest -r linux-x64 --self-contained /p:FunckyDisableAnalyzers=true
dotnet publish -c Release Funcky.TrimmingTest -r linux-x64 --self-contained /p:FunckyDisableAnalyzers=true /p:FunckyTestAot=true
```

Documentation (mdBook) lives in `Documentation/`: `mdbook serve Documentation`.

## Multi-targeting — the central constraint

`Funcky` targets a wide TFM range: `net10.0;net9.0;net8.0;net7.0;net6.0;net5.0;netcoreapp3.1;netstandard2.0;netstandard2.1`.
You cannot assume any modern BCL API is available. Newer-runtime features are gated behind
preprocessor symbols defined in `FrameworkFeatureConstants.props` (e.g. `DATE_ONLY_SUPPORTED`,
`GENERIC_MATH`, `RANGE_SUPPORTED`, `ORDERED_DICTIONARY`). When using an API that isn't on every
target, wrap it in `#if SOME_CONSTANT` and add the constant to `FrameworkFeatureConstants.props`
keyed to the minimum compatible framework. `PolySharp` polyfills language-feature attributes so
modern C# can be used even on `netstandard2.0`.

**Dependency policy:** the core `Funcky` package must have no dependencies. The only exceptions are
Microsoft backwards-compat shims already shipped in newer frameworks (e.g.
`Microsoft.Bcl.HashCode`, `System.Collections.Immutable`, `System.Text.Json`), added conditionally
for the older TFMs only. Interop with other libraries goes in separate packages
(`Funcky.Async`, `Funcky.Xunit`, etc.).

## Public API tracking — required for any API change

`Funcky` uses `Microsoft.CodeAnalysis.PublicApiAnalyzers`. Every public surface change must be
recorded in `Funcky/PublicAPI.Unshipped.txt` (and `PublicAPI.Shipped.txt` on release), or the build
fails. The analyzer only runs for the newest TFM (`FunckyNewestTargetFramework`), because APIs are
often added on newer frameworks but not older ones. User-visible changes also belong in `changelog.md`.

## Documentation — keep it in sync

The mdBook in `Documentation/` is part of the deliverable, not an afterthought. Update it in the same
change that alters behavior:

- **Analyzer rules:** every diagnostic gets a page `Documentation/src/analyzer-rules/λNNNN.md` following
  the existing structure (title, Cause, Reason for rule, How to fix violations, Examples with
  Disallowed/Allowed). Add it to the table in `analyzer-rules/analyzer-rules.md` and to `SUMMARY.md`.
  Removed rules move to the "Removed rules" table.
- **New or changed public API:** extend the matching chapter (`option.md`, `enumerable-extensions/`,
  `functional-helpers/`, …) or add a new page and link it from `SUMMARY.md`. Unlinked pages are not
  built by mdBook.
- Examples in the docs must compile against the current API; verify names against the source rather
  than writing them from memory.

## Source generators

`Funcky.SourceGenerator` generates code at build time; prefer extending it over hand-writing
repetitive members:

- **`[OrNoneFromTryPattern(typeof(T), nameof(T.TryParse))]`** on a `partial class` generates
  `…OrNone` methods that wrap a BCL `Try*` pattern into an `Option`-returning method. See
  `Funcky/Extensions/ParseExtensions/*` — e.g. `ParseExtensions.Numbers.cs` declares the attributes
  and the bodies are generated.
- **Apply generator** generates the `Apply`/currying overloads.

## Code conventions

- **One method (group) per file, in `partial` classes.** Big static classes like
  `EnumerableExtensions`, `Sequence`, `Option`, and `ParseExtensions` are split across many files
  named after the operation (`Sequence.Return.cs`, `Option.Monad.cs`, `Pairwise.cs`). Add new
  members as another `partial` file following the same naming.
- **`[Pure]`** annotates pure methods (`System.Diagnostics.Contracts` is a global using).
- **Implicit/global usings** are enabled. `Funcky.Extensions`, `Funcky.Monads`, and static
  `Funcky.Functional` are globally imported (`GlobalUsings.props`); don't add redundant `using`s.
  Consumers get the same via the packaged `build/Funcky.targets` when `FunckyImplicitUsings` is on.
- **Nullable** reference types are enabled everywhere; `LangVersion` is `preview`.
- Style is enforced by `Polyadic.CodeStyle` + `.editorconfig` (4-space indent, LF, UTF-8;
  tabs for `.props`/`.targets`/`.slnx`).
- Spelling is CI-checked via `typos` (`typos.toml`).

## Test-driven development

Follow TDD for changes to library behavior: write the failing test first, then the implementation.

- Add a failing test in the matching test project (`Funcky.Test`, `Funcky.Async.Test`,
  `Funcky.Analyzers.Test`, `Funcky.SourceGenerator.Test`, …) before writing the production code,
  mirroring the one-operation-per-file layout of the code under test.
- Run it and confirm it fails for the expected reason, then implement the minimal code to make it
  pass (`dotnet test <project> --filter "FullyQualifiedName~<Name>"`).
- Prefer FsCheck property-based tests for laws and invariants (purity, monad laws, lazy/streaming
  behavior); use Verify snapshot tests for source-generator output and analyzer diagnostics.
- Refactor only once green, keeping the suite passing on all target frameworks.

## Project layout

- `Funcky/` — the core library. `Monads/` (Option, Result, Either, Reader, Lazy), `Extensions/`
  (Enumerable, AsyncEnumerable, Parse, String, …), `Sequence/` (IEnumerable constructors),
  `Functional/` (Identity, etc.), `RetryPolicies/`, `Internal/`.
- `Funcky.Async/` — async variants (`IAsyncEnumerable`, async Option). Separate package so core stays dependency-free.
- `Funcky.Xunit/`, `Funcky.Xunit.v3/` — assertion helpers for Funcky types (xUnit v2 and v3).
- `Funcky.Analyzers/` — Roslyn analyzers + code fixes guiding correct Funcky usage.
  Analyzers and code fixes are in **separate projects** (code fixes need
  `Microsoft.CodeAnalysis.*.Workspaces`, which isn't available during `dotnet build`).
  `BuiltinAnalyzers` are mandatory analyzers shipped inside the `Funcky` package itself.
  Analyzers are Roslyn-multi-targeted (`roslyn4.0` / `roslyn4.12` directories) — see
  `Funcky.Analyzers/readme.md` before touching analyzer packaging.
- `Funcky.SourceGenerator/` — incremental source generators (see above).
- `Funcky.Test/`, `Funcky.Async.Test/`, `Funcky.Analyzers.Test/`, `Funcky.SourceGenerator.Test/`,
  `Funcky.Xunit*.Test/` — tests. They use xUnit, FsCheck (property-based), and Verify (snapshot /
  source-generator output). Test global usings live in `GlobalUsings.Test.props`.
- `Funcky.FsCheck/` — F# project for property-based tests.
- `Documentation/` — mdBook source.

Central build configuration lives in `Directory.Build.props`, `Directory.Packages.props`
(central package version management — add versions there, not in `.csproj`), `Analyzers.props`,
`FrameworkFeatureConstants.props`, `GlobalUsings*.props`, and `PublicApiAnalyzers.targets`.
