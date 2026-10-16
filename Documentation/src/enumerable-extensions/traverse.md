## Traverse

`Select` with a monadic selector, followed by `Sequence`: apply a function that may fail to every element, and
get either all the results or the first failure.

```cs
Option<IReadOnlyList<TItem>> Traverse<TSource, TItem>(this IEnumerable<TSource> source, Func<TSource, Option<TItem>> selector)
Result<IReadOnlyList<TValidResult>> Traverse<TSource, TValidResult>(this IEnumerable<TSource> source, Func<TSource, Result<TValidResult>> selector)
Either<TLeft, IReadOnlyList<TRight>> Traverse<TSource, TLeft, TRight>(this IEnumerable<TSource> source, Func<TSource, Either<TLeft, TRight>> selector)
Reader<TEnvironment, IEnumerable<TResult>> Traverse<TSource, TEnvironment, TResult>(this IEnumerable<TSource> source, Func<TSource, Reader<TEnvironment, TResult>> selector)
Lazy<IEnumerable<T>> Traverse<TSource, T>(this IEnumerable<TSource> source, Func<TSource, Lazy<T>> selector)
```

<picture>
    <picture>
      <source srcset="traverse-dark.svg" media="(prefers-color-scheme: dark)">
      <img src="traverse.svg" alt="A marble diagram showing the Traverse operation">
    </picture>
</picture>

Everything said about [`Sequence`](#sequence) applies: all or nothing for `Option`, `Result` and `Either`, with
enumeration stopping at the first failure; deferral for `Reader` and `Lazy`. `Traverse` just saves the
intermediate sequence of monads.

This is the operation to reach for when a validation or lookup must succeed for every element before anything
is done with the results:

### Example

```cs
Option<IReadOnlyList<int>> ParseAll(IEnumerable<string> input)
    => input.Traverse(ParseExtensions.ParseInt32OrNone);

ParseAll(Sequence.Return("1", "2", "3"));   // Some([1, 2, 3])
ParseAll(Sequence.Return("1", "x", "3"));   // None
```

```cs
Result<IReadOnlyList<Order>> PlaceAll(IEnumerable<OrderRequest> requests)
    => requests.Traverse(PlaceOrder);   // the first Error wins; later requests are not attempted

Either<ValidationError, IReadOnlyList<Record>> ValidateAll(IEnumerable<string> lines)
    => lines.Traverse(ValidateLine);
```
