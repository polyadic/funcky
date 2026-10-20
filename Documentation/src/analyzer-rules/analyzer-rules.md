# Analyzer Rules
Funcky ships two sets of Roslyn analyzers that guide you towards correct and idiomatic usage of the library.
One written rule decides where a rule lives:

* **Built-in analyzers** are part of the `Funcky` package itself and are always active.
  A rule is built in when the flagged code is *outside the supported surface* of the library:
  the member only exists to support a compiler feature, the value is one the type system permits but Funcky does not
  support, or the code can never do what it says. Such code is wrong, not merely unidiomatic, so these rules report
  **errors** and carry the category `Funcky.Correctness`.
* **[`Funcky.Analyzers`]** is an optional package with additional rules.
  A rule lives there when the flagged code *works*, but Funcky offers a more idiomatic, shorter or cheaper form.
  These rules report **warnings**, always come with a code fix and carry the category `Funcky.Style`. A rule that is
  purely a performance hint, such as [λ1015](./λ1015.md), reports an **info** instead. Install the package with:
  ```xml
  <PackageReference Include="Funcky.Analyzers" Version="..." PrivateAssets="all" />
  ```

This is the same split the .NET libraries themselves use: usage diagnostics ship inside packages such as
`System.Text.Json` and are always on, while best-practice hints like `xunit.analyzers` are a package of their own.

### Rule numbers
Rule IDs are stable identifiers: once a rule has shipped, its number never changes, because projects configure and
suppress rules by ID. The number range therefore records where a rule was *born*, not where it lives today:
`λ0xxx` rules started as built-in rules, `λ1xxx` rules started in `Funcky.Analyzers`, and `λ11xx` rules concern the
`FunctionalAssert` helpers from the [`Funcky.Xunit`] package. The tables below are the authority on which package
contains a rule.

## Built-in rules

| Rule | Severity | Title |
|------|----------|-------|
| [λ0001](./λ0001.md) | Error | Disallowed use of `TryGetValue` |
| [λ0003](./λ0003.md) | Error | Member is not intended for direct usage |
| [λ0004](./λ0004.md) | Error | The `default` literal is wrapped by an implicit conversion |
| [λ0005](./λ0005.md) | Error | A `null` literal is passed to `Option`, `Result` or `Either` |
| [λ1009](./λ1009.md) | Error | Do not use `default` to instantiate this type (moved from `Funcky.Analyzers` in 3.7.0) |
| [λ1010](./λ1010.md) | Error | An option has either zero or one elements (needs Roslyn 4.4, i.e. the .NET 7 SDK or newer) |

## Funcky.Analyzers

| Rule | Severity | Title |
|------|----------|-------|
| [λ1001](./λ1001.md) | Warning | Use of `Enumerable.Repeat` for a single element |
| [λ1002](./λ1002.md) | Warning | Use of `Enumerable.Repeat` with no element |
| [λ1003](./λ1003.md) | Warning | Use this method with argument names |
| [λ1004](./λ1004.md) | Warning | Use `ConcatToString` instead of `JoinToString(string.Empty)` |
| [λ1005](./λ1005.md) | Warning | Prefer `GetOrElse` over `Match` |
| [λ1006](./λ1006.md) | Warning | Prefer `OrElse` over `Match` |
| [λ1007](./λ1007.md) | Warning | Prefer `SelectMany` over `Match` |
| [λ1008](./λ1008.md) | Warning | Prefer `ToNullable` over `Match` |
| [λ1014](./λ1014.md) | Warning | Throw expression in an eagerly evaluated fallback |
| [λ1015](./λ1015.md) | Info | Fallback is computed eagerly |
| [λ1016](./λ1016.md) | Warning | Pass the fallback value directly |
| [λ1017](./λ1017.md) | Warning | Prefer the named function over the lambda |
| [λ1018](./λ1018.md) | Warning | Prefer `Flatten` over `SelectMany` with the identity function |
| [λ1101](./λ1101.md) | Warning | Assert can be simplified |

## Removed rules

| Rule | Title | Note |
|------|-------|------|
| λ0002 | Use of `Option<T>.None()` method group | `Option<T>.None()` was replaced by the `Option<T>.None` property in 3.0. The rule and its code fix were removed once the method was gone. See the [migration guide](../migration-guide.md). |

## Using the attributes in your own code
Two rules are driven by attributes: [λ1003](./λ1003.md) looks for `[UseWithArgumentNames]` on methods and
[λ1009](./λ1009.md) for `[NonDefaultable]` on structs. The attributes are internal to Funcky, but the analyzers match them
by their full name in the `Funcky.CodeAnalysis` namespace, so a copy in any assembly counts. `Funcky.Analyzers` can
generate that copy into your project; list the attributes you want in the project file:

```xml
<PropertyGroup>
  <FunckyCodeAnalysisAttributes>UseWithArgumentNames;NonDefaultable</FunckyCodeAnalysisAttributes>
</PropertyGroup>
```

The generated attributes are `internal`, so projects that need them generate their own. The generation is opt-in because
a project that already has its own copy of an attribute would otherwise get a duplicate type. An unknown name in the list
produces the build warning `λ.BUILD.0002`.

## Configuring severity
All rules can be configured in an `.editorconfig` like any other Roslyn diagnostic, for example:

```ini
[*.cs]
dotnet_diagnostic.λ1003.severity = suggestion
```

The categories let you configure a whole set at once. This turns every style rule into a suggestion while the
correctness rules stay errors:

```ini
[*.cs]
dotnet_analyzer_diagnostic.category-Funcky.Style.severity = suggestion
```

The only exception is [λ0003](./λ0003.md), which is not configurable because the flagged members do not work outside their intended syntax.

## Opting out of the built-in analyzers
The built-in analyzers are meant to stay on. For build environments where they cannot run, set the MSBuild property
`FunckyDisableBuiltinAnalyzers` to `true` in the project or in `Directory.Build.props`; the package's targets then
remove the analyzer assemblies from the build. Excluding the analyzer assets on the package reference has the same
effect:

```xml
<PackageReference Include="Funcky" Version="..." ExcludeAssets="analyzers" />
```

[`Funcky.Analyzers`]: https://www.nuget.org/packages/Funcky.Analyzers
[`Funcky.Xunit`]: https://www.nuget.org/packages/Funcky.Xunit
