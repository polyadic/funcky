## MinOrNone

Returns the smallest element of a sequence, or `None` if the sequence is empty.

```cs
Option<TSource> MinOrNone<TSource>(this IEnumerable<TSource> source)
Option<TSource> MinOrNone<TSource>(this IEnumerable<Option<TSource>> source)
Option<TResult> MinOrNone<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
Option<TResult> MinOrNone<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, Option<TResult>> selector)
```

<picture>
    <picture>
      <source srcset="min-or-none-dark.svg" media="(prefers-color-scheme: dark)">
      <img src="min-or-none.svg" alt="A marble diagram showing the MinOrNone operation">
    </picture>
</picture>

The BCL's `Min` returns `null` for an empty sequence of nullable values but throws for an empty sequence of
`int`. `MinOrNone` treats both the same: empty is `None`. Elements are compared with `Comparer<T>.Default`, and
when several elements are equal the first one wins.

The overloads taking `Option<T>` elements, or a selector returning `Option<T>`, skip the `None` elements, in the
same way the BCL's `Min` skips `null`. A sequence consisting only of `None` is therefore empty and yields `None`.

### Example

```cs
var numbers = Sequence.Return(3, 1, 4, 1, 5);

numbers.MinOrNone();                       // Some(1)
Enumerable.Empty<int>().MinOrNone();       // None
numbers.MinOrNone(n => n * -1);            // Some(-5)

var readings = Sequence.Return(Option.Some(3), Option<int>.None, Option.Some(2));
readings.MinOrNone();                      // Some(2)
```
