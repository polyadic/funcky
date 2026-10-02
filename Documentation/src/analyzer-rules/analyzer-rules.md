# Analyzer Rules
Funcky ships two sets of Roslyn analyzers that guide you towards correct and idiomatic usage of the library.

* **Built-in analyzers** (`λ0xxx`) are part of the `Funcky` package itself and are always active.
  They guard against usages that we consider incorrect or unsafe.
* **[`Funcky.Analyzers`]** (`λ1xxx`) is an optional package with additional style and usage rules,
  most of them with an automatic code fix. Install it with:
  ```xml
  <PackageReference Include="Funcky.Analyzers" Version="..." PrivateAssets="all" />
  ```

Rules in the `λ11xx` range concern the `FunctionalAssert` helpers from the [`Funcky.Xunit`] package.

## Built-in rules

| Rule | Severity | Title |
|------|----------|-------|
| [λ0001](./λ0001.md) | Error | Disallowed use of `TryGetValue` |
| [λ0003](./λ0003.md) | Error | Member is not intended for direct usage |

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
| [λ1009](./λ1009.md) | Error | Do not use `default` to instantiate this type |
| [λ1010](./λ1010.md) | Error | An option has either zero or one elements |
| [λ1101](./λ1101.md) | Warning | Assert can be simplified |

## Removed rules

| Rule | Title | Note |
|------|-------|------|
| λ0002 | Use of `Option<T>.None()` method group | `Option<T>.None()` was replaced by the `Option<T>.None` property in 3.0. The rule and its code fix were removed once the method was gone. See the [migration guide](../migration-guide.md). |

## Configuring severity
All rules can be configured in an `.editorconfig` like any other Roslyn diagnostic, for example:

```ini
[*.cs]
dotnet_diagnostic.λ1003.severity = suggestion
```

The only exception is [λ0003](./λ0003.md), which is not configurable because the flagged members do not work outside their intended syntax.

[`Funcky.Analyzers`]: https://www.nuget.org/packages/Funcky.Analyzers
[`Funcky.Xunit`]: https://www.nuget.org/packages/Funcky.Xunit
