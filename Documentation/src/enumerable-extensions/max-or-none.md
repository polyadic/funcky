## MaxOrNone

Returns the largest element of a sequence, or `None` if the sequence is empty.

```cs
Option<TSource> MaxOrNone<TSource>(this IEnumerable<TSource> source)
Option<TSource> MaxOrNone<TSource>(this IEnumerable<Option<TSource>> source)
Option<TResult> MaxOrNone<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
Option<TResult> MaxOrNone<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, Option<TResult>> selector)
```

<picture>
    <picture>
      <source srcset="max-or-none-dark.svg" media="(prefers-color-scheme: dark)">
      <img src="max-or-none.svg" alt="A marble diagram showing the MaxOrNone operation">
    </picture>
</picture>

`MaxOrNone` mirrors `MinOrNone` in every respect: empty is `None`, comparison uses `Comparer<T>.Default`, the
first of several equal elements wins, and `None` elements are skipped.

### Example

```cs
var numbers = Sequence.Return(3, 1, 4, 1, 5);

numbers.MaxOrNone();                       // Some(5)
Enumerable.Empty<int>().MaxOrNone();       // None
numbers.MaxOrNone(n => n % 3);             // Some(2)
```
