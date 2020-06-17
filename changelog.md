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
* Added overload for AndThen which flattens the Option
* Add `Where` method to `Option<T>`, which allows filtering the `Option` by a predicate.
* Add overload for `Option<T>.SelectMany` that takes only a selector.

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

## Unreleased
* Move the `Ok` constructor of `Result<T>` to a non-generic class. This allows for the compiler to infer the generic type.
  Old: `Result<int>.Ok(10)`. New: `Result.Ok(10)`.
* Add `IndexOfOrNone`, `LastIndexOfOrNone`, `IndexOfAnyOrNone` and `LastIndexOfAnyOrNone` extension methods to `string`.
* Rename `OrElse` overloads that return the item to `GetOrElse` which improves overload resolution.
* Added `Curry`, `Uncurry` and `Flip` to the `Functional` Class
