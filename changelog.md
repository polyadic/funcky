# Changelog
All notable changes to this project will be documented in this file.
Funcky adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## Unreleased
* The behaviour of the `Result.Error` constructor has been changed regarding exceptions
  with an already set stack trace. The original stack trace is now preserved.
  Previously this resulted in the stacktrace being replaced (.NET < 5.0) or an error (.NET ≥ 5.0).
* Added extensions `DequeueOrNone` and `PeekOrNone` on `Queue` and `ConcurrentQueue`
* `Sequence.Generate` has been deprecated in favour of the newly added `Sequence.Successors` function
  which includes the first element (seed) in the generated sequence.
* Extension functions for `System.Range` to allow the generations of `IEnumerable<T>`s from Range-Syntax:

```cs
  foreach(var i in 1..5) { }

  // negative numbers are not supported
  from x in 5..2
  from y in 1..3
  select (x,y)
```

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
