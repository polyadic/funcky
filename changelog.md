# Changelog

## 1.6.0
* Add ToEnumerable function to `Option<T>`.
* Add `WhereSelect` extension function for `IEnumerable<T>`.
* Add missing overload for nullary actions to `ActionToUnit`.

## 1.7.0
* Add nullability annotations to everything except for `Monads.Reader`.
* Add a function for creating an `Option<T>` from a nullable value: `Option.From`.
* `Either.Match` now throws when called on an `Either` value created using `default(Either<L, R>)`.
* Add `True` and `False` functions to public API
* Match of `Result` Monad accepts actions
* Add `FirstOrNone`, `LastOrNone` and `SingleOrNone` extension functions

## 1.8.0
* Added overload for `AndThen` which flattens the `Option`
* Add `Where` method to `Option<T>`, which allows filtering the `Option` by a predicate.
* Add overload for `Option<T>.SelectMany` that takes only a selector.
* Add `WhereNotNull` extension method for `IEnumerable<T>`.

## 2.0.0-rc.1
* Full nullable support introduced with C# 8
* Rename `Option.From` -> `Option.FromNullable` and remove overload that takes non-nullable value types.
* Use `Func<T, bool>` instead of `Predicate<T>` in predicate composition functions (`Functional.All`, `Functional.Any`, `Functional.Not`),
  because most APIs in `System` use `Func`.
* `Functional.Any` now returns `false` when the given list of predicates is empty.
* The `Each` extension method on `IEnumerable<T>` has been renamed to `ForEach`.
* Unify `Option<T>.ToEnumerable` and `Yield` to `ToEnumerable`
* Remove `Reader` monad based on `await`.
* `Exception` created by `Result` monad contains valid stack trace

## 2.0.0-rc.2
* Move the `Ok` constructor of `Result<T>` to a non-generic class. This allows for the compiler to infer the generic type.
  Old: `Result<int>.Ok(10)`. New: `Result.Ok(10)`.
* Add `IndexOfOrNone`, `LastIndexOfOrNone`, `IndexOfAnyOrNone` and `LastIndexOfAnyOrNone` extension methods to `string`.
* Rename `OrElse` overloads that return the item to `GetOrElse` which improves overload resolution.
* Added `Curry`, `Uncurry` and `Flip` to the `Functional` Class
* Remove `IToString`.
* Mark our functions as `[Pure]`.
* Fix incorrect implementation on `Result.SelectMany` which called the `selectedResultSelector` even when the
  result was an error. As a result (pun intended) of the fix, `ResultCombinationException` is no longer needed and also removed.

## 2.0.0

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

## Funcky 2.1.0 | Funcky.xUnit 0.1.1
* Add `Inspect` method to `Option` akin to `IEnumerable.Inspect`.
* Add `ToTheoryData` extension for `IEnumerable<T>` for xUnit.
* Add `Unit.Value` as a way to a get a `Unit` value.
* Add `Functional.Retry` which retries a producer until `Option.Some` is returned.

## Funcky 2.1.1 | Funcky.xUnit 0.1.2
* Re-release of previous release with correct assemblies.

## Funcky 2.2.0 | Funcky.xUnit 0.1.3
* Added overload to `Functional.Retry` with a `IRetryPolicy`.
* Added `None` overload that takes no predicate.

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

### Improvements
* `ConfigureAwait(false)` is now used everywhere `await` is used.
* The `IRetryPolicy` implementations now use correct `Timespan` with `double` multiplication
  when targeting .NET Standard 2.0.

### Deprecations
* `ObjectExtensions.ToEnumerable` has been deprecated in favor of `Sequence.FromNullable`.
* `RequireClass` and `RequireStruct` have been obsoleted with no replacement.
