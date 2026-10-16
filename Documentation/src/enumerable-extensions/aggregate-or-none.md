## AggregateOrNone

Folds a sequence with an accumulator function, using the first element as the seed, or returns `None` if the
sequence is empty.

```cs
Option<TSource> AggregateOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, TSource> func)
```

This is the `…OrNone` variant of the seedless `Aggregate`, which throws an `InvalidOperationException` for an empty
sequence. The seeded overloads of `Aggregate` have no such problem, because the seed is the answer for an empty
sequence, so they have no `…OrNone` variant. For the same reason there is no `SumOrNone`: the sum of nothing is zero.

The accumulator is applied from the left and is never invoked for a sequence with a single element; that element
is the result.

### Example

```cs
var words = Sequence.Return("functional", "programming", "in", "C#");

words.AggregateOrNone((longest, word) => word.Length > longest.Length ? word : longest);   // Some("programming")
Enumerable.Empty<string>().AggregateOrNone((a, b) => a + b);                             // None
```
