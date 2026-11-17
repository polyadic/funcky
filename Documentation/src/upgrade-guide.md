# Upgrade Guide

This chapter is the one place to look when you move a project from one major version of Funcky to the next. Each
section lists what changed, what you have to do about it, and which analyzer code fix does the work for you. The
[changelog](./changelog.md) has the full list of changes per release; this chapter only covers the ones that break a
build or change behaviour.

The sections are ordered newest first.

## From 3.x to 4.0

Funcky 4.0.0 ships together with Funcky.Async 2.0.0, Funcky.Xunit 3.0.0, Funcky.Xunit.v3 2.0.0 and Funcky.Analyzers
3.0.0. It removes everything that 3.8.0 obsoleted, drops the target frameworks that are out of support, and builds on
the .NET 11 SDK.

### Prepare on 3.8.0, then upgrade

There is no 3.9 release to carry obsoletions, so the upgrade has two steps, and the first one happens before you touch
the version number:

1. Update to Funcky 3.8.0 (Funcky.Async 1.6.0, Funcky.Xunit 2.3.0, Funcky.Xunit.v3 1.1.0, Funcky.Analyzers 2.1.0)
   and reference Funcky.Analyzers if you do not already.
2. In `.editorconfig`, raise the rules that point at the classic forms to a warning, so that their code fixes apply:

   ```ini
   dotnet_diagnostic.λ1201.severity = warning   # text.ParseInt32OrNone() -> int.ParseOrNone(text), optional
   dotnet_diagnostic.λ1202.severity = warning   # Sequence.Return(x) -> Enumerable.Return(x)
   dotnet_diagnostic.λ1203.severity = warning   # FunctionalAssert.Some(x, o) -> Assert.Some(x, o)
   ```

3. Apply the code fixes with the fix-all scope *solution*: λ1202, λ1203, and the fixes for the two obsoletion warnings
   (CS0618) on `Option.FromBoolean` and on the `…Await` / `…AwaitAsync` async operators. One pass per fix migrates
   every call site. λ1201 is optional; the `string` form stays in 4.0.
4. Build, fix what is left by hand (the few cases listed below), then move to 4.0.0.

If you skip the preparation, the same code fixes still fire on 4.0.0, now on the compile errors the removed members
leave behind (CS0117 for `Option.FromBoolean`, CS1061 for the async operators); λ1202 and λ1203 are gone in
Funcky.Analyzers 3.0.0, so `Sequence` and `FunctionalAssert` calls have to be rewritten by hand in that order.

### Target frameworks and SDK

* **Removed target frameworks:** `net7.0`, `net6.0`, `net5.0` and `netcoreapp3.1`, all out of Microsoft's support. Funcky
  ships `net11.0`, `net10.0`, `net9.0`, `net8.0`, `netstandard2.1` and `netstandard2.0`; Funcky.Async drops its `net5.0`
  asset; Funcky.Xunit and Funcky.Xunit.v3 replace their `net6.0` asset with `net8.0`. A project on a removed framework
  still restores, but gets the `netstandard2.1` asset (or `netstandard2.0` below .NET Core 3.0), which lacks the members
  that need a newer base class library: the generic math and `IParsable` based parsers, `DateOnly`/`TimeOnly`, the
  trimming annotations. Retarget to .NET 8 or newer.
* **Analyzers need Roslyn 4.14:** the built-in analyzers and Funcky.Analyzers 3.0.0 ship a single `roslyn4.14` asset,
  which loads on Visual Studio 2022 17.14 and on the .NET SDK 9.0.300 or newer, so on every .NET 10 SDK. An older SDK
  silently skips the analyzers; the library itself keeps working on it.
* **Extension members need the C# 14 compiler:** the `Assert.Some` assertions, the `Enumerable.Return` constructors and
  the static `int.ParseOrNone` parsers are static extension members. They exist as plain static methods on an older
  compiler (`EnumerableExtensions.Return(x)`, `Int32ParseExtensions.ParseOrNone(text)`), but the .NET 10 SDK is the
  practical baseline for Funcky 4.

### Removed members

| Removed | Use instead | Code fix |
|---------|-------------|----------|
| `Option.FromBoolean(condition)`, `FromBoolean(condition, value)`, `FromBoolean(condition, () => value)` | `condition.Then(Unit.Value)`, `condition.Then(value)`, `condition.Then(() => value)` | Funcky.Analyzers, on CS0618 (3.8.0) and CS0117 (4.0.0) |
| `Funcky.Sequence.Return`, `Successors`, `Cycle`, `CycleRange`, `CycleMaterialized`, `RepeatRange`, `RepeatMaterialized`, `FromNullable`, `Concat` | The same names as static extension members of `System.Linq.Enumerable`: `Enumerable.Return(42)`, declared in `Funcky.Extensions.EnumerableExtensions` | λ1202 on 3.8.0 only |
| `Funcky.FunctionalAssert.Some`, `None`, `Ok`, `Error`, `Left`, `Right` (Funcky.Xunit 3.0.0, Funcky.Xunit.v3 2.0.0) | The same names as static extension members of xunit's `Assert`: `Assert.Some(42, option)`, declared in `Funcky.Extensions.AssertExtensions` | λ1203 on the last 2.x release only |
| `…Await` and `…AwaitWithCancellation` operators (`AdjacentGroupBy`, `Chunk`, `ExclusiveScan`, `InclusiveScan`, `Inspect`, `WhereSelect`) and the `…AwaitAsync` / `…AwaitWithCancellationAsync` terminal aggregations (`AverageOrNone`, `FirstOrNone`, `LastOrNone`, `SingleOrNone`, `MaxOrNone`, `MinOrNone`, `None`, `Partition`) | The single name with a `(item, cancellationToken)` delegate: `xs.Inspect((x, _) => …)` | Funcky.Analyzers, on CS0618 (3.8.0) and CS1061 (4.0.0) |

The async code fix is mechanical: `…AwaitWithCancellation` and `…AwaitWithCancellationAsync` calls are renamed and the
delegate is left alone; `…Await` and `…AwaitAsync` calls are renamed and every lambda gets a discarded token parameter,
`x => …` becomes `(x, _) => …`; a method group or a delegate value becomes a forwarding lambda, `xs.InspectAwait(Log)`
becomes `xs.Inspect((x, _) => Log(x))`. Two shapes get no fix and need a hand: a delegate produced by an expression that
would change meaning when evaluated per element (`xs.InspectAwait(CreateInspector())`, store it in a local first), and
anonymous methods in the `delegate (x) { … }` syntax (convert them to lambdas). `RetryAwaitAsync` in `AsyncFunctional`
is a different method that happens to share the infix and stays.

The failure message label of the assertions changes with the removal of `FunctionalAssert`: `FunctionalAssert.Some()
Failure` becomes `Assert.Some() Failure`, matching xunit's own `Assert.Equal() Failure`. A test that asserts on the
message text needs the new label.

### `Result.Match` and `Result.Switch` take the error case first

`Match(ok, error)` is now `Match(error, ok)`, and `Switch(ok, error)` is `Switch(error, ok)`, consistent with
`Option.Match(none, some)` and `Either.Match(left, right)`. Both methods carry `[UseWithArgumentNames]`, so a call that
follows [λ1003](./analyzer-rules/λ1003.md) and names its arguments is unaffected. A positional call fails to compile
after the upgrade, because the two delegates have different parameter types: swap its arguments. The `Match` rules of
Funcky.Analyzers (λ1005 to λ1007 and λ1011) tell the two arguments apart by parameter name and work with either order.

### Funcky.Async 2.0 and System.Linq.Async

Funcky.Async 2.0.0 no longer depends on System.Linq.Async but on
[System.Linq.AsyncEnumerable](https://www.nuget.org/packages/System.Linq.AsyncEnumerable) `[10.0.6, 12)`, the package
that is the `IAsyncEnumerable` LINQ of the .NET 10 base class library, so the operators have the same shapes on every
framework. Two members that only bridged System.Linq.Async 5 and 6 are gone: `Chunk(size)` without a result selector
(System.Linq.AsyncEnumerable provides it, yielding `T[]`), and `AdjacentGroupBy` now returns
`IAsyncEnumerable<IGrouping<TKey, T>>` instead of `IAsyncGrouping` on every framework.

A project that still references System.Linq.Async 5 or 6 next to Funcky.Async 2.0.0 gets a type clash (CS0433) on
`System.Linq.AsyncEnumerable`. Upgrade that reference to System.Linq.Async 7, which forwards to the same package, or
drop it. A project that cannot stays on Funcky.Async 1.x, which receives no new features.

### Analyzer rules

Funcky.Analyzers 3.0.0 retires [λ1202](./analyzer-rules/analyzer-rules.md#removed-rules) and λ1203, because the forms
they pointed away from no longer exist; their numbers stay reserved. λ1201 stays as an opt-in style rule, because both
parse forms remain. The C# 14 language gate of the `λ12xx` rules is gone: Funcky 4 assumes the .NET 10 SDK, whose
compiler always binds the extension forms.

### Funcky.DiscriminatedUnion is retired

[Funcky.DiscriminatedUnion](https://github.com/polyadic/funcky-discriminated-union) receives no further changes; its
final 1.2.x version stays on NuGet for the 3.x line. C# has native `union` types and `closed` hierarchies since .NET 11,
which cover what the generator did. Replace `[DiscriminatedUnion]` on the abstract base with `closed` and the generated
`Match` with a `switch` expression over the cases, which the compiler checks for exhaustiveness:

```cs
// Funcky 3.x with Funcky.DiscriminatedUnion
[DiscriminatedUnion]
public abstract partial record Shape
{
    public sealed partial record Circle(double Radius) : Shape;
    public sealed partial record Rectangle(double Width, double Height) : Shape;
}

var area = shape.Match(
    circle: c => Math.PI * c.Radius * c.Radius,
    rectangle: r => r.Width * r.Height);

// Funcky 4 on .NET 11: a closed hierarchy and a switch expression
public closed record Shape
{
    public sealed record Circle(double Radius) : Shape;
    public sealed record Rectangle(double Width, double Height) : Shape;
}

var area = shape switch
{
    Shape.Circle c => Math.PI * c.Radius * c.Radius,
    Shape.Rectangle r => r.Width * r.Height,
};
```

A `closed` hierarchy is implicitly abstract and its derived types must live in the same assembly, which is what the
generator enforced as well. A missing case in the `switch` is the compiler warning CS8509 instead of a generated
`Match` parameter you cannot omit. Projects that stay below .NET 11 keep using the generator on the 3.x line. See
[#900](https://github.com/polyadic/funcky/issues/900).

## From 2.x to 3.0

Funcky 3.0.0 shipped together with Funcky.Async 1.0.0 and Funcky.Xunit 2.0.0.

1. Update to the latest 2.x version of Funcky (2.7.1) and fix all warnings. Every API that 3.0 removes is obsolete
   there: `ObjectExtensions.ToEnumerable`, `RequireClass` and `RequireStruct` in `Funcky.GenericConstraints`, all `Try…`
   APIs (`TryGetValue`, `TryParse…`, which use the `…OrNone` suffix instead), `Sequence.Generate` (superseded by
   `Sequence.Successors`) and `CartesianProduct`.
2. Update to Funcky 3.0.
3. Run `dotnet format analyzers`. This migrates `Option<T>.None()` to the `Option<T>.None` property for you (rule λ0002,
   removed again once the method was gone). The command sometimes fails while loading your project(s); `--no-restore`
   might help with that.
4. Build and fix the remaining compilation failures:
   * The `Match` overloads taking an `Action` are renamed to `Switch`.
   * `EnumerableExtensions.ForEach` returns `Unit`.
   * Many parameter names and generic type names were unified across Funcky; a call that names its arguments may need
     the new names.
   * All `Action` extensions moved to `ActionExtensions`, `EitherOrBoth` to the `Funcky` namespace, the retry policies
     to `Funcky.RetryPolicies`.
   * `Partition` returns a `Partitions` struct instead of a tuple.
   * The parse extensions use BCL type names (`ParseInt32OrNone` instead of `ParseIntOrNone`) and the parameter names
     and nullability of the BCL.
   * `Result<T>`, `Either<L, R>` and `Reader<E, R>` disallow `null` values, as `Option<T>` always did.
   * The materializing `IEnumerable<T>` extensions (`Materialize`, `Chunk`, `Partition`, `Shuffle`, `SlidingWindow`,
     `Split`, `Transpose`, `Sequence.Return`) return `IReadOnlyList<T>` or `IReadOnlyCollection<T>`.
5. You might need to re-run `dotnet format analyzers` after fixing other errors.
6. **Important:** check whether you serialize an `Option` with `System.Text.Json`. The implicit converter is gone; you
   need to register the [`OptionJsonConverter`](./option.md) yourself. You get no compile error for this, only a runtime
   failure, so test it.
7. Everything related to `IAsyncEnumerable` and `Task` moved to the new **Funcky.Async** package:
   `AsyncEnumerableExtensions`, `Functional.RetryAsync` (now `AsyncFunctional.RetryAsync`), and the `Option<Task>` and
   `Option<ValueTask>` awaiters. Add the package reference where you use them.

## Within the 3.x line

Minor releases do not break a build, but two of them changed what you see from the analyzers:

* **3.7.0 (Funcky.Analyzers 2.0.0):** λ1009 "Do not use `default` to instantiate this type" and λ1010 "An option has
  either zero or one elements" moved from Funcky.Analyzers into the analyzers built into Funcky, so every consumer gets
  them; a project that references an older Funcky.Analyzers next to Funcky 3.7.0 reports λ1009 twice until it updates.
  The diagnostic category of every rule changed from `Funcky` to `Funcky.Correctness` (built-in) or `Funcky.Style`
  (Funcky.Analyzers), which matters to `.editorconfig` entries that configure a category. Funcky.Analyzers 2.0.0 dropped
  its Roslyn 4.0 asset.
* **3.8.0:** the extension methods of Funcky, Funcky.Async and Funcky.Xunit became C# 14 extension members, and the
  extension forms that 4.0 makes the only ones arrived next to the classic API: `Enumerable.Return`, `Assert.Some`,
  `int.ParseOrNone`. Funcky.Analyzers 2.1.0 ships λ1201 to λ1203 to move to them; see the 4.0 section above.
