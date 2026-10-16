## Sequence

Turns a sequence of monadic values inside out: an `IEnumerable<Option<T>>` becomes an `Option<IReadOnlyList<T>>`,
and likewise for `Result<T>`, `Either<L, R>`, `Reader<E, T>` and `Lazy<T>`.

```cs
Option<IReadOnlyList<TSource>> Sequence<TSource>(this IEnumerable<Option<TSource>> source)
Result<IReadOnlyList<TSource>> Sequence<TSource>(this IEnumerable<Result<TSource>> source)
Either<TLeft, IReadOnlyList<TSource>> Sequence<TLeft, TSource>(this IEnumerable<Either<TLeft, TSource>> source)
Reader<TEnvironment, IEnumerable<TSource>> Sequence<TEnvironment, TSource>(this IEnumerable<Reader<TEnvironment, TSource>> sequence)
Lazy<IEnumerable<TSource>> Sequence<TSource>(this IEnumerable<Lazy<TSource>> sequence)
```

For the three failure monads the semantics are "all or nothing": the result is `Some`, `Ok` or `Right` with
the list of all inner values if every element succeeded, and the *first* `None`, `Error` or `Left` otherwise.
The input is enumerated only up to that first failure, and the method is eager.

For `Reader` and `Lazy` there is no failure, only deferral: the result is a reader that, given an environment,
runs every inner reader, or a lazy value that evaluates every inner lazy. These two are lazy themselves.

`Sequence` differs from [`WhereSelect`](./filtering-and-partitioning.md#whereselect) and
[`Partition`](./filtering-and-partitioning.md#partition) in what a failure means. `WhereSelect` drops the failed
elements and keeps going. `Partition` keeps both the failures and the successes. `Sequence` says that one
failure fails the whole thing.

The name comes from Haskell, where `sequence` is the general operation of swapping two type constructors.
The inverse direction, swapping a monad with a sequence inside it, lives on the monad types themselves; see the
[Result chapter](../result.md#combining-with-other-monads).

### Example

```cs
var allParsed = Sequence.Return("1", "2", "3").Select(ParseExtensions.ParseInt32OrNone).Sequence();
// Some([1, 2, 3])

var oneBad = Sequence.Return("1", "x", "3").Select(ParseExtensions.ParseInt32OrNone).Sequence();
// None
```

`Select` followed by `Sequence` is so common that it has its own name, [`Traverse`](#traverse).
