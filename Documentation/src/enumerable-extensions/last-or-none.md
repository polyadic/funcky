## LastOrNone

Returns the last element of a sequence, or `None` if the sequence is empty. With a predicate, returns the last
element that satisfies it, or `None` if no element does.

```cs
Option<TSource> LastOrNone<TSource>(this IEnumerable<TSource> source)
Option<TSource> LastOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
```

<picture>
    <picture>
      <source srcset="last-or-none-dark.svg" media="(prefers-color-scheme: dark)">
      <img src="last-or-none.svg" alt="A marble diagram showing the LastOrNone operation">
    </picture>
</picture>

Unlike `FirstOrNone`, `LastOrNone` has to enumerate the whole sequence before it can answer, so it never
returns on an infinite sequence.

### Example

```cs
var numbers = Sequence.Return(3, 1, 4, 1, 5);

numbers.LastOrNone();               // Some(5)
numbers.LastOrNone(n => n < 3);     // Some(1)
numbers.LastOrNone(n => n > 10);    // None
```
