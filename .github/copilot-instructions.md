# Funcky — Copilot Instructions

Funcky is a functional programming library for C#. It encourages idiomatic, side-effect-free
functional C# by leaning on the language's existing monadic interfaces (LINQ, async) rather than
inventing a parallel world. The centerpiece is the `Option` monad; the library also ships
`Result`, `Either`, `Reader`, and `Lazy` monads, a `Unit` type, fundamental functions
(`Identity`, `True`, `False`, …), `IEnumerable` constructors (`Sequence.Return`,
`Sequence.Successors`, …), and a large set of `IEnumerable`/`string`/`IQueryable` extensions.

## Multi-targeting is the central constraint

`Funcky` multi-targets a wide TFM range:
`net10.0;net9.0;net8.0;net7.0;net6.0;net5.0;netcoreapp3.1;netstandard2.0;netstandard2.1`.

- **Do not assume a modern BCL API is available.** Newer-runtime features are gated behind
  preprocessor symbols defined in `FrameworkFeatureConstants.props` (e.g. `DATE_ONLY_SUPPORTED`,
  `GENERIC_MATH`, `RANGE_SUPPORTED`, `ORDERED_DICTIONARY`).
- When using an API that isn't on every target, wrap it in `#if SOME_CONSTANT` and add the
  constant to `FrameworkFeatureConstants.props`, keyed to the minimum compatible framework.
- `PolySharp` polyfills language-feature attributes, so modern C# syntax can be used even on
  `netstandard2.0`.
- `LangVersion` is `preview` and nullable reference types are enabled everywhere.

## Dependency policy

- The core `Funcky` package **must have no dependencies.** The only exceptions are Microsoft
  backwards-compat shims already shipped in newer frameworks (e.g. `Microsoft.Bcl.HashCode`,
  `System.Collections.Immutable`, `System.Text.Json`), added conditionally for the older TFMs only.
- Interop with other libraries goes in **separate packages** (`Funcky.Async`, `Funcky.Xunit`, …).
- Package versions are managed centrally in `Directory.Packages.props` — add versions there, not in
  individual `.csproj` files.

## Public API tracking — required for any public API change

`Funcky` uses `Microsoft.CodeAnalysis.PublicApiAnalyzers`. Every public surface change must be
recorded in `Funcky/PublicAPI.Unshipped.txt` or the build fails. The analyzer only runs for the
newest TFM (`FunckyNewestTargetFramework`). User-visible changes also belong in `changelog.md`.

## Source generators — prefer extending over hand-writing

`Funcky.SourceGenerator` generates repetitive members at build time:

- **`[OrNoneFromTryPattern(typeof(T), nameof(T.TryParse))]`** on a `partial class` generates
  `…OrNone` methods that wrap a BCL `Try*` pattern into an `Option`-returning method. See
  `Funcky/Extensions/ParseExtensions/*` (e.g. `ParseExtensions.Numbers.cs`).
- The **Apply generator** generates the `Apply`/currying overloads.

## Code conventions

- **One method (group) per file, in `partial` classes.** Big static classes like
  `EnumerableExtensions`, `Sequence`, `Option`, and `ParseExtensions` are split across many files
  named after the operation (`Sequence.Return.cs`, `Option.Monad.cs`, `Pairwise.cs`). Add new
  members as another `partial` file following the same naming.
- **`[Pure]`** annotates pure methods (`System.Diagnostics.Contracts` is a global using).
- **Implicit/global usings** are enabled. `Funcky.Extensions`, `Funcky.Monads`, and static
  `Funcky.Functional` are globally imported (`GlobalUsings.props`); don't add redundant `using`s.
- Style is enforced by `Polyadic.CodeStyle` + `.editorconfig`: 4-space indent, LF, UTF-8 for `.cs`;
  **tabs** for `.props`/`.targets`/`.slnx`. Snapshot `*.verified.cs` files keep trailing whitespace
  and have no final newline.
- Spelling is CI-checked via `typos` (`typos.toml`).

## Build & test

There is no `.sln`; the solution file is `Funcky.slnx` (XML-based). `dotnet` commands operate on it
automatically from the repo root.

```bash
dotnet restore
dotnet build                       # CI builds Release with /p:TreatWarningsAsErrors=true
dotnet test                        # runs all test projects
dotnet test Funcky.Test            # one project
dotnet test Funcky.Test --filter "FullyQualifiedName~Pairwise"   # subset by name
dotnet test --framework net10.0    # pin one target framework
```

- CI builds on Linux and Windows with `TreatWarningsAsErrors=true`, so **a warning is a build
  failure**. Tests also run against two `System.Linq.Async` versions (`SystemLinqAsyncVersion`).
- Trimming / AOT compatibility is enforced via `Funcky.TrimmingTest` (`dotnet publish … -r linux-x64
  --self-contained`, optionally `/p:FunckyTestAot=true`).
- Documentation (mdBook) lives in `Documentation/`: `mdbook serve Documentation`.

## Test-driven development

Follow TDD for changes to library behavior: write the failing test first, then the implementation.

- Add a failing test in the matching test project (`Funcky.Test`, `Funcky.Async.Test`,
  `Funcky.Analyzers.Test`, `Funcky.SourceGenerator.Test`, …), mirroring the one-operation-per-file
  layout, and confirm it fails for the expected reason before implementing.
- Prefer **FsCheck** property-based tests for laws and invariants (purity, monad laws,
  lazy/streaming behavior); use **Verify** snapshot tests for source-generator output and analyzer
  diagnostics. Test global usings live in `GlobalUsings.Test.props`.
- Refactor only once green, keeping the suite passing on all target frameworks.

## Project layout

- `Funcky/` — the core library: `Monads/` (Option, Result, Either, Reader, Lazy), `Extensions/`
  (Enumerable, AsyncEnumerable, Parse, String, …), `Sequence/` (IEnumerable constructors),
  `Functional/` (Identity, etc.), `RetryPolicies/`, `Internal/`.
- `Funcky.Async/` — async variants (`IAsyncEnumerable`, async Option); separate package so the core
  stays dependency-free.
- `Funcky.Xunit/`, `Funcky.Xunit.v3/` — assertion helpers for Funcky types (xUnit v2 and v3).
- `Funcky.Analyzers/` — Roslyn analyzers + code fixes. Analyzers and code fixes live in **separate
  projects** (code fixes need `Microsoft.CodeAnalysis.*.Workspaces`, unavailable during
  `dotnet build`). `BuiltinAnalyzers` are mandatory analyzers shipped inside the `Funcky` package.
  Analyzers are Roslyn-multi-targeted (`roslyn4.0` / `roslyn4.12`) — read
  `Funcky.Analyzers/readme.md` before touching analyzer packaging.
- `Funcky.SourceGenerator/` — incremental source generators (see above).
- `Funcky.FsCheck/` — F# project for property-based tests.
- `Documentation/` — mdBook source.

Central build configuration lives in `Directory.Build.props`, `Directory.Packages.props`,
`Analyzers.props`, `FrameworkFeatureConstants.props`, `GlobalUsings*.props`, and
`PublicApiAnalyzers.targets`.
