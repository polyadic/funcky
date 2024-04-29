# Changelog
All notable changes to this project will be documented in this file.
Funcky adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## Funcky 3.4.0 | Funcky.Async 1.3.0 | Funcky.XUnit 2.0.2

This update is mainly to update to .NET 8 but also has several smaller improvements.

### .NET 8

We use the new C#12 and .NET features in the code, and expose new features through our API.

* .NET 8 added new overloads to their `TryParse` APIs. These changes  are reflected in Funcky's `ParseOrNone` APIs.
  * `ParseByteOrNone` overloads with `ReadOnlySpan<byte>` and `string?`
  * `ParseSByteOrNone` overloads with `ReadOnlySpan<byte>`
  * `ParseSingleOrNone` overloads with `ReadOnlySpan<byte>`
  * `ParseDoubleOrNone` overloads with `ReadOnlySpan<byte>`
  * `ParseDecimalOrNone` overloads with `ReadOnlySpan<byte>`
  * `ParseInt16OrNone` overloads with `ReadOnlySpan<byte>`
  * `ParseInt32OrNone` overloads with `ReadOnlySpan<byte>`
  * `ParseInt64OrNone` overloads with `ReadOnlySpan<byte>`
  * `ParseUInt16OrNone` overloads with `ReadOnlySpan<byte>`
  * `ParseUInt32OrNone` overloads with `ReadOnlySpan<byte>`
  * `ParseUInt64OrNone` overloads with `ReadOnlySpan<byte>`
  * `ParseNumberOrNone<TNumber>` overloads
  * `ParseOrNone<TParsable>` overloads

### String Extensions

We implemented a few of the IEnumerable extensions which are very useful on strings.

* `Chunk` on `string`.
* `SlidingWindow` on `string`.

### Monads

* Implemented `UpCast` for the monds `Option`, `Either`, `Result` and `System.Lazy`.
* Implemented `OrElse` on `Either`
* Implemented `OrElse` and `GetOrElse` on `Result`
* Implemented `InspectLeft` on `Either`
* Implemented `InspectNone` on `Option`
* Implemented `InspectError` on `Result`
* Implemented `InspectEmpty` on `IEnumerable` and `IAEnumerable`
* Implemented `ToAsyncEnumerable` extension on `Option`

### IEnumerator 

* `MoveNextOrNone` extention on `IEnumerator<T>`

### Consistency

* `FindIndexOrNone` and `FindLastIndexOrNone` extensions on `List`


## Funcky 3.3.0 | Funcky.Analyzers 1.3.0 | Funcky.Xunit 2.0.1
This is a relatively minor release focuses on convenience for our monads `Option`, `Either` and `Result`.

### `GetOrElse` and `OrElse` for all
We've added `GetOrElse` and `OrElse` to `Either` and `Result` bringing them on par with `Option`. \
The corresponding analyzer now also correctly suggests using these methods instead of `Match` for `Result` and `Either`.

### `Inspect` for the error case
All three alternative monads `Option`, `Either` and `Result` now support inspecting the «error» case:
* `Option.InspectNone` - executes a side effect only when the option is `None`.
* `Either.InspectLeft` - executes a side effect only when the either is on the `Left` side.
* `Result.InspectError` - executes a side effect only when the result is an `Error`.

These methods are particularly useful for logging warnings/errors.

### Funcky.XUnit

* Funcky.XUnit is only compatible with XUnit 2.4, this is now correctly declared.

## Funcky 3.2.0 | Funcky.Async 1.2.0
### List Pattern for Option
We've added support for C# 11's List Patterns to `Option<T>`.
This means that you can use regular `switch` expressions / statements to match on options:

```cs
var greeting = person switch
{
    { FirstName: var firstName, LastName: [var lastName] } => $"Hello {firstName} {lastName}",
    { FirstName: var firstName } => $"Hi {firstName}",
};

record Person(string FirstName, Option<string> LastName);
```

### Discard
The new `Discard.__` field provides a short-hand for `Unit.Value` to be used with `switch` expressions.

```cs
using static Funcky.Discard;

return __ switch
{
    _ when user.IsFrenchAdmin() => "le sécret",
    _ when user.IsAdmin() => "secret",
    _ => "(redacted)",
};
```

### Retry with Exception
We've added overloads to the `Retry` and `RetryAsync` functions that allow retrying a function
as long as an exception is thrown.

Example from [`IoRetrier`](https://github.com/messerli-informatik-ag/io/blob/main/IO/Retrier.cs):
```cs
// Retries an action until the file is no longer in use.
Task RetryWhileFileIsInUseAsync(IRetryPolicy policy, Action action)
    => RetryAsync(ActionToUnit(action), exception => exception is IOException && exception.HResult == FileInUseHResult, policy);
```

### New Extensions
* `EnumerableExtensions.MinByOrNone`
* `EnumerableExtensions.MaxByOrNone`
* `ImmutableListExtensions.IndexOfOrNone`
* `ImmutableListExtensions.LastIndexOfOrNone`
* `ListExtensions.IndexOfOrNone`

## Funcky 3.1.0 | Funcky.Async 1.1.0 | Funcky.Analyzers 1.2.0
### New APIs
* ✨ `OptionExtensions.ToNullable` ✨
* `StreamExtenions.ReadByteOrNone`
* New overloads for `ElementAtOrNone` that take an `Index`.
* New overload for `JoinToString` that takes an `IEnumerable<string>`.

### .NET 7
* .NET 7 added new overloads to their `TryParse` APIs. These changes
   are reflected in Funcky's `ParseOrNone` APIs.
* The `ParseOrNone` methods include the new `[StringSyntax]` attribute from .NET 7.

### Analyzers
The new `Option.Match` analyzer suggests simpler alternatives over custom `Match`es including
the all-new `ToNullable` extension.

## Funcky 3.0.0 | Funcky.Async 1.0.0 | Funcky.XUnit 2.0.0
There's a handy [Migration Guide](https://polyadic.github.io/funcky/migration-guide.html) available.

### New APIs
* `Result.GetOrThrow`
* `EnumerableExtensions.GetNonEnumeratedCountOrNone`

#### `PriorityQueue`
* `PriorityQueueExtensions.DequeueOrNone`
* `PeekOrNone`

#### Traversable
The new `Traverse` and `Sequence` extension methods allow you to
«swap» the inner and outer monad (e.g. `Result<Option<T>>` -> `Option<Result<T>>`)

#### `Memoize`
The new `Memoize` extension function returns an `IBuffer` / `IAsyncBuffer`. \
This new type represents ownership over the underlying enumerator (and is therefore `IDisposable`).

`CycleRange` and `RepeatRange` have also been changed to return an `IBuffer`.

### `ParseExtensions`
The parse extensions have been improved with the goal of aligning more with the BCL.
Many of the changes are breaking.

* The functions now use BCL type names instead of C# type names
  (e.g. `ParseIntOrNone` has been renamed to `Parse`)
* The parameter names and nullability have been changed to align with the BCL.
* Added `HttpHeadersNonValidatedExtensions`

### `IReadOnlyList` / `IReadOnlyCollection`
Funcky now communicates materialization in the `IEnumerable<T>` extensions by returning
`IReadOnlyList` or `IReadOnlyCollection`. This reduces «multiple enumeration» warnings.

* `Materialize`
* `Chunk`
* `Partition`
* `Shuffle`
* `SlidingWindow`
* `Split`
* `Transpose`
* `Sequence.Return`

### Disallowing `null` Values
Our `Option<T>` type has always disallowed `null` values.
This has been extended to our other monads: `Result<T>`, `Either<L, R>` and `Reader<E, R>`.

### Breaking Changes
* `Option.None()` has been changed to a property. There's an automatic fix available for this.
* Our `Match` functions now differentiate between `Func` and `Action`.
  The `Action` overloads have been renamed to `Switch`.
* The return type of `EnumerableExtensions.ForEach` has been changed to `Unit`.
* Many parameter names and generic type names have been renamed in an attempt to unify naming across Funcky.
* All `Action` extensions have been moved to a new class `ActionExtensions`.
* `EitherOrBoth` has been moved to the `Funcky` namespace.
* The retry policies have been moved to the `Funcky.RetryPolicies` namespace.
* `Partition` returns a custom `Partitions` struct instead of a tuple.

#### Obsoleted APIs Removed
APIs that have been obsoleted during 2.x have been removed:

* `ObjectExtensions.ToEnumerable`
* `Funcky.GenericConstraints.RequireClass` and `RequireStruct`
* All `Try*` APIs (`TryGetValue`, `TryParse*`, etc.). These APIs use the `OrNone` suffix instead.
* `Sequence.Generate` has been superceded by `Sequence.Successors`
* `CartesianProduct`

#### JSON Converter
We have removed the implicit `System.Text.Json` converter for `Option<T>`.
This means that you'll have to register the `OptionJsonConverter` yourself. \
⚠️ Test this change carefully as you won't get an error during compilation, but rather at runtime.

#### Moved to Funcky.Async
All APIs related to `IAsyncEnumerable` and `Task` have been moved to the new `Funcky.Async` package:

* `AsyncEnumerableExtensions`
* `Functional.RetryAsync` -> `AsyncFunctional.RetryAsync`
* `Option<Task>` and `Option<ValueTask>` awaiters

### Funcky.Async
#### `AsyncSequence`
This class exposes all of the same factory functions as `Sequence`, but for `IAsyncEnumerable`:
* `Return`
* `Successors`
* `Concat`
* `Cycle`
* `CycleRange`
* `FromNullable`
* `RepeatRange`

#### New `IAsyncEnumerable` extensions
We've worked hard towards the goal of parity between our extensions for `IEnumerable` and `IAsyncEnumerable`:

* `AdjacentGroupBy`
* `AnyOrElse`
* `AverageOrNoneAsync` / `MaxOrNoneAsync` / `MinOrNoneAsync`
* `Chunk`
* `ConcatToStringAsync`
* `ExclusiveScan`
* `InclusiveScan`
* `Inspect`
* `Interleave`
* `Intersperse`
* `JoinToStringAsync`
* `MaterializeAsync`
* `Memoize`
* `Merge`
* `NoneAsync`
* `PartitionAsync`
* `PowerSet`
* `Sequence` / `SequenceAsync` / `Traverse` / `TraverseAsync`
* `ShuffleAsync`
* `SlidingWindow`
* `Split`
* `Transpose`
* `WhereNotNull`
* `WithIndex` / `WithLast` / `WithPrevious` / `WithFirst`
* `ZipLongest`

### Funcky.Xunit
* Breaking: The `Is` prefix has been dropped from assertion methods for consistency with
XUnit's naming scheme for assertion methods.

## Funcky 2.7.1
### Deprecations
* `Option.None<T>()`: We originally introduced the `Option.None<T>` method as a future proof replacement to `Option<T>.None` for use in method groups,
  because Funcky 3 changes `Option<T>.None` to a property. This turned out to be confusing to users especially because both method are always suggested in autocomplete.

## Funcky 2.7.0 | Funcky.XUnit 1.0.0 | Funcky.Analyzers 1.1.0
This release is the last non-breaking release for Funcky before 3.0.

### Deprecations
* `EnumerableExtensions.CartesianProduct` will be removed in Funcky 3.
* To align our naming with that of the BCL, the `ParseOrNone` methods
  that return a type that has a keyword in C# `int`, `long`, etc. use the name of the BCL type instead. \
  Example: `ParseIntOrNone` becomes `ParseInt32OrNone`. \
  The old methods will be removed in Funcky 3.
* In preparation for Funcky 3 we deprecated `Option<T>.None` when used as method group. Use `Option.None<T>` instead.

### New `ParseOrNone` extensions
With the help of a source generator we have added a lot of new ParseOrNone methods for various types from the BCL:
  * Unsigned integer types
  * `DateOnly`, `TimeOnly`
  * `Version`
  * Support for `ReadOnlySpan<T>` as input
  * ... and more

### Convenience for `Either` and `Result`
* Added implicit conversions for `Either` and `Result`.
* Implement `Inspect` for `Either` and `Result`.
* Added `Partition` for `IEnumerable<Either>` and `IEnumerable<Result>`.
* Added `ToString` on `Either` and `Result`.
* Implement `ToEither` on `Option`.

### `IEnumerable<T>` extensions
* `AnyOrElse`
* Prefix sum: `InclusiveScan` and `ExclusiveScan`

### Analyzers
This release adds two new analyzer rules:

* λ1003: Warning when certain methods, such as `Match` are used without argument labels
* λ1004: Warning that suggests `.ConcatToString()` over `.JoinToString("")`

Both of these warnings come with corresponding code fixes.

### Funcky.Xunit
* Breaking: Funcky.Xunit now uses the `Funcky` namespace, instead of `Funcky.Xunit`.
* Add assertion methods for testing `Either`: `FunctionalAssert.IsLeft` and `FunctionalAssert.IsRight`.

## Funcky 2.6.0 | Funcky.Analyzers 1.0.0
### Analyzers
This release comes with a new package `Funcky.Analyzers`, which we'll use
to guide users of Funcky

### New extensions
* Add extensions `DequeueOrNone` and `PeekOrNone` on `Queue` and `ConcurrentQueue`.
* Add extension `ConcatToString` as an alias for `string.Concat`.
* Add overload to `WhereSelect` with no parameter.
* Add methods to convert from `Either` to `Option`: [#439](https://github.com/polyadic/funcky/issues/439)
    * `LeftOrNone`: Returns the left value or `None` if the either value was right.
    * `RightOrNone`: Returns the right value or `None` if the either value was left.
* Extension functions for `System.Range` to allow the generations of `IEnumerable<T>`s from Range-Syntax:
  ```cs
  foreach(var i in 1..5) { }

  // negative numbers are not supported
  from x in 5..2
  from y in 1..3
  select (x, y)
  ```

### Improvements to `Sequence`
* `Sequence.Return` now accepts multiple parameters:
  ```cs
  Sequence.Return(1, 2, 3)
  ```
* ⚠️ `Sequence.Generate` has been deprecated in favour of the newly added `Sequence.Successors` function
  which includes the first element (seed) in the generated sequence.

### Improvements to `Option`
* Add `Option.FromBoolean` to create an `Option<T>` from a boolean.

### Improvements to `Result`
The behaviour of the `Result.Error` constructor has been changed regarding exceptions
with an already set stack trace. The original stack trace is now preserved.
Previously this resulted in the stacktrace being replaced (.NET < 5.0) or an error (.NET ≥ 5.0).

### Improvements to `Either`
* Add `Either.Flip` to swaps left with right.

### Tooling
* Funcky automatically adds global usings for the most important namespaces of funcky
  when the `FunckyImplicitUsings` property is set. This requires .NET SDK ≥ 6.0 and C# ≥ 10.0.
* Funcky now supports [trimming](https://docs.microsoft.com/en-us/dotnet/core/deploying/trimming/trim-self-contained) for self-contained deployments.
* `Option<T>` now works with the new [System.Text.Json source generation](https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-source-generation?pivots=dotnet-6-0).
* The `Funcky` package now supports [Source Link](https://docs.microsoft.com/en-us/dotnet/standard/library-guidance/sourcelink) and deterministic builds.
* The symbols package is now finally working again.

## Funcky 2.5.0
### Reader Monad
This release includes the `Reader` monad including a bunch of factory methods
and convenience extensions.

```cs
public static Reader<Enviroment, IEnumerable<string>> DefaultLayout(IEnumerable<DateTime> month)
    => from colorizedMonthName in ColorizedMonthName(month)
       from weekDayLine in WeekDayLine()
       from weeksInMonth in month
        .GroupBy(GetWeekOfYear)
        .Select(FormatWeek)
        .Sequence()
       select BuildDefaultLayout(colorizedMonthName, weekDayLine, weeksInMonth);
```

### Improved `Action` Extensions
Funcky now supports `Curry`, `Uncurry` and `Flip` for `Action`s too. \
This release also adds the inversion of `ActionToUnit`: `UnitToAction`

### More Extensions for `IEnumerable<T>`
* `Intersperse`: Adds a given item in between all items of an enumerable.
* `JoinToString`: Alias for `string.Join`.
* `WithPrevious`: Similar to `WithFirst/Last/Index` but with the predecessor of each item.
* `ForEach`: Add an overload to `ForEach` that accepts a `Unit`-returning `Func`.

### Additional Factory Methods
* `EitherOrBoth.FromOptions` creates an `EitherOrBoth` from two options.
* `Lazy.FromFunc` creates a `Lazy<T>` from a `Func`. \
   This is sugar over the `Lazy<T>` constructor, with the additional benefit of supporting type inference.
* `Lazy.Return` creates a `Lazy<T>` from a value. \
  This is sugar over the `Lazy<T>` constructor, with the additional benefit of supporting type inference.

### Documentation Improvements
This release comes with a few small documentation improvements.
Funcky users will now also see the `[Pure]` attributes which were previously not emitted.

## Funcky 2.4.1
* Remove upper bounds on all Microsoft.Bcl.\* dependencies.
  Between the 2.3.0 and 2.4.0 release an overly restrictive upper bound was accidentally introduced for Microsoft.Bcl.AsyncInterfaces.

## Funcky 2.4.0
### `Try*` → `*OrNone`
We've renamed all `Try*` methods, such as `TryParse`, `TryGet` value to `*OrNone`.
The old methods are still available, but marked as obsolete and will be removed in 3.0.0.

### Factory methods for `IEnumerable<T>`
This release adds some new factory methods for creating `IEnumerable<T>`
to the `Sequence` class:
* `Sequence.RepeatRange`: Generates a sequence that contains the same sequence of elements the given number of times
* `Sequence.Cycle`: Cycles the same element over and over again as an endless generator.
* `Sequence.CycleRange`: Generates a sequence that contains the same sequence of elements over and over again as an endless generator
* `Sequence.Concat`

### More Extension Methods
#### for `IEnumerable<T>`
  * `Materialize`: Materializes all the items of a lazy enumerable.
  * `PowerSet`: Returns a sequence with the set of all subsets
  * `Shuffle`: Returns the given sequence in random Order in O(n).
  * `Split`: Splits the source sequence a separator.
  * `ZipLongest`: Zips two sequences with different lengths.
#### for `string`
* `SplitLazy`: Splits a string by separator lazily.
* `SplitLines`: Splits a string by newline lazily.
#### for `Func`
* `Curry`
* `Uncurry`
* `Flip`
* `Compose`

### `EitherOrBoth`
EitherOrBoth is a new data type that can represent `Left`, `Right` and `Both`. It is used in `ZipLongest`.

### `Monad.Return`
This release adds a `Return` method for all monad types in Funcky:
* `Option.Return`
* `Either<TLeft>.Return`
* `Result.Return`

### `OptionEqualityComparer`
To support more advanced comparison scenarios, `OptionEqualityComparer` has been added similar to the already existing `OptionComparer`.

### Smaller Improvements
* Added a missing `Match` overload to `Either` that takes `Action`s
* Added additional overloads for `Functional.True` and `Functional.False` for up to four parameters.

## Funcky 2.3.0
* `net5.0` has been added to Funcky's target frameworks.

### Improvements to `Option<T>`
* `Option<T>` is now implicitly convertible from `T`.
  ```csharp
  public static Option<int> Answer => 42;
  ```
* `Option` adds support for `System.Text.Json`:\
  The custom `JsonConverter` is picked up automatically when serializing/deserializing.
  `None` is serialized as `null` and `Some(value)` is serialized to whatever `value` serializes to.

### Factory methods for `IEnumerable<T>`
This release adds factory methods for creating `IEnumerable<T>`
with the static class `Sequence`:
* `Sequence.Return`: Creates an `IEnumerable<T>` with exactly one item.
* `Sequence.FromNullable`: Creates an `IEnumerable<T>` with zero or one items.
* `Sequence.Generate`: Creates an `IEnumerable<T>` using a generation function and a seed.

### More Extension Methods for `IEnumerable<T>`
This release adds a bunch of new extension methods on `IEnumerable<T>`:
* `AdjacentGroupBy`
* `AverageOrNone`
* `CartesianProduct`
* `Chunk`
* `ElementAtOrNone`
* `Interleave`
* `MaxOrNone`
* `Merge`
* `MinOrNone`
* `Pairwise`
* `Partition`
* `SlidingWindow`
* `TakeEvery`
* `Transpose`
* `WithFirst`
* `WithIndex`
* `WithLast`

### `IAsyncEnumerable<T>` Support
This release adds a couple of extension methods that provide interoperability
with `Option<T>` to `IAsyncEnumerable<T>`:
* `WhereSelect`
* `FirstOrNoneAsync`
* `LastOrNoneAsync`
* `SingleOrNoneAsync`
* `ElementAtOrNoneAsync`

A couple of the new extension methods on `IEnumerable<T>` have async counterparts:
* `Pairwise`
* `TakeEvery`

The naming of the extension methods and their overloads follows that of [`System.Linq.Async`](https://github.com/dotnet/reactive/tree/main/Ix.NET/Source/System.Linq.Async).

### Improved `IQueryable` Support
This release adds specialized extension methods for `IQueryable<T>` that are better
suited especially for use with EF Core:
* `FirstOrNone`
* `LastOrNone`
* `SingleOrNone`

### Dependencies
To support .NET Standard, Funcky conditionally pulls in dependencies
that provide the missing functionality:
* `Microsoft.Bcl.AsyncInterfaces` for .NET Standard 2.0
* `System.Collections.Immutable` and `System.Text.Json` for .NET Standard 2.0 and 2.1
* The version constraints for all these packages have been relaxed to allow 5.x.

### Improvements
* `ConfigureAwait(false)` is now used everywhere `await` is used.
* The `IRetryPolicy` implementations now use correct `Timespan` with `double` multiplication
  when targeting .NET Standard 2.0.

### Deprecations
* `ObjectExtensions.ToEnumerable` has been deprecated in favor of `Sequence.FromNullable`.
* `RequireClass` and `RequireStruct` have been obsoleted with no replacement.

## Funcky 2.2.0 | Funcky.xUnit 0.1.3
* Added overload to `Functional.Retry` with a `IRetryPolicy`.
* Added `None` overload that takes no predicate.

## Funcky 2.1.1 | Funcky.xUnit 0.1.2
* Re-release of previous release with correct assemblies.

## Funcky 2.1.0 | Funcky.xUnit 0.1.1
* Add `Inspect` method to `Option` akin to `IEnumerable.Inspect`.
* Add `ToTheoryData` extension for `IEnumerable<T>` for xUnit.
* Add `Unit.Value` as a way to a get a `Unit` value.
* Add `Functional.Retry` which retries a producer until `Option.Some` is returned.

## Funcky 2.0.0
### Breaking Changes
* Remove `Reader` monad based on `await`.
* Remove `IToString`.
* Remove overload for `Option.From` that flattens passed `Option`s.
* Move `ToEnumerable` extension method to its own class.
  This is only a breaking change if you've used the extension method as normal method.
  In that case you need to change `EnumerableExtensions.ToEnumerable` to `ObjectExtensions.ToEnumerable`.
* Rename `Option.From` to `Option.FromNullable` and remove overload that takes non-nullable value types.
* Unify `Option<T>.ToEnumerable` and `Yield` to `ToEnumerable`
* Rename `OrElse` overloads that return the item to `GetOrElse` which improves overload resolution.
* The `Each` extension method on `IEnumerable<T>` has been renamed to `ForEach`.
* Move the `Ok` constructor of `Result<T>` to a non-generic class. This allows for the compiler to infer the generic type.
  Old: `Result<int>.Ok(10)`. New: `Result.Ok(10)`.
* Use `Func<T, bool>` instead of `Predicate<T>` in predicate composition functions (`Functional.All`, `Functional.Any`, `Functional.Not`),
  because most APIs in `System` use `Func`.
* `Functional.Any` now returns `false` when the given list of predicates is empty.

### Fixes
* Fix incorrect `Equals` implementation on `Option`.
  `Equals` previously returned `true` when comparing a `None` value with a `Some` value containing the default value of the type.
* `Exception` created by `Result` monad contains valid stack trace
* Fix incorrect implementation on `Result.SelectMany` which called the `selectedResultSelector` even when the
  result was an error. As a result (pun intended) of the fix, `ResultCombinationException` is no longer needed and also removed.

### Additions
* Add `IndexOfOrNone`, `LastIndexOfOrNone`, `IndexOfAnyOrNone` and `LastIndexOfAnyOrNone` extension methods to `string`.
* Added `Curry`, `Uncurry` and `Flip` to the `Functional` Class
* Add extension method for `HttpHeaders.TryGetValues`, which returns an `Option`.
* Add extension methods for getting `Stream` properties that are not always available, as `Option`:
  `GetLengthOrNone`, `GetPositionOrNone`, `GetReadTimeoutOrNone`, `GetWriteTimeoutOrNone`.
* Add `None` extension method to `IEnumerable`.
* `Option<Task<T>>`, `Option<Task>` and their `ValueTask` equivalents are now awaitable:
  ```csharp
  var answer = await Option.Some(Task.FromResult(42));
  ```

### Improvements
* Full nullable support introduced with C# 8.
* Mark our functions as `[Pure]`.
* Implement `IEquatable` on `Option`, `Result` and `Either`.

## Funcky 2.0.0-rc.2
* Move the `Ok` constructor of `Result<T>` to a non-generic class. This allows for the compiler to infer the generic type.
  Old: `Result<int>.Ok(10)`. New: `Result.Ok(10)`.
* Add `IndexOfOrNone`, `LastIndexOfOrNone`, `IndexOfAnyOrNone` and `LastIndexOfAnyOrNone` extension methods to `string`.
* Rename `OrElse` overloads that return the item to `GetOrElse` which improves overload resolution.
* Added `Curry`, `Uncurry` and `Flip` to the `Functional` Class
* Remove `IToString`.
* Mark our functions as `[Pure]`.
* Fix incorrect implementation on `Result.SelectMany` which called the `selectedResultSelector` even when the
  result was an error. As a result (pun intended) of the fix, `ResultCombinationException` is no longer needed and also removed.

## Funcky 2.0.0-rc.1
* Full nullable support introduced with C# 8
* Rename `Option.From` -> `Option.FromNullable` and remove overload that takes non-nullable value types.
* Use `Func<T, bool>` instead of `Predicate<T>` in predicate composition functions (`Functional.All`, `Functional.Any`, `Functional.Not`),
  because most APIs in `System` use `Func`.
* `Functional.Any` now returns `false` when the given list of predicates is empty.
* The `Each` extension method on `IEnumerable<T>` has been renamed to `ForEach`.
* Unify `Option<T>.ToEnumerable` and `Yield` to `ToEnumerable`
* Remove `Reader` monad based on `await`.
* `Exception` created by `Result` monad contains valid stack trace

## Funcky 1.8.0
* Added overload for `AndThen` which flattens the `Option`
* Add `Where` method to `Option<T>`, which allows filtering the `Option` by a predicate.
* Add overload for `Option<T>.SelectMany` that takes only a selector.
* Add `WhereNotNull` extension method for `IEnumerable<T>`.

## Funcky 1.7.0
* Add nullability annotations to everything except for `Monads.Reader`.
* Add a function for creating an `Option<T>` from a nullable value: `Option.From`.
* `Either.Match` now throws when called on an `Either` value created using `default(Either<L, R>)`.
* Add `True` and `False` functions to public API
* Match of `Result` Monad accepts actions
* Add `FirstOrNone`, `LastOrNone` and `SingleOrNone` extension functions

## Funcky 1.6.0
* Add ToEnumerable function to `Option<T>`.
* Add `WhereSelect` extension function for `IEnumerable<T>`.
* Add missing overload for nullary actions to `ActionToUnit`.
