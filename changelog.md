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
