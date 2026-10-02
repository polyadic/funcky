## ElementAtOrNone

Returns the element at the given position, or `None` if the position is outside the sequence.

```cs
Option<TSource> ElementAtOrNone<TSource>(this IEnumerable<TSource> source, int index)
Option<TSource> ElementAtOrNone<TSource>(this IEnumerable<TSource> source, Index index)   // .NET 6+
```

<picture>
    <picture>
      <source srcset="element-at-or-none-dark.svg" media="(prefers-color-scheme: dark)">
      <img src="element-at-or-none.svg" alt="A marble diagram showing the ElementAtOrNone operation">
    </picture>
</picture>

A negative index is not an error, it is simply not in the sequence and yields `None`. The `Index` overload
accepts positions from the end (`^1` is the last element); counting from the end may require enumerating the
whole sequence.

### Example

```cs
var numbers = Sequence.Return(3, 1, 4, 1, 5);

numbers.ElementAtOrNone(2);     // Some(4)
numbers.ElementAtOrNone(5);     // None
numbers.ElementAtOrNone(-1);    // None
numbers.ElementAtOrNone(^1);    // Some(5)
```
