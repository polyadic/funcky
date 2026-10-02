## FirstOrNone

Returns the first element of a sequence, or `None` if the sequence is empty. With a predicate, returns the first
element that satisfies it, or `None` if no element does.

```cs
Option<TSource> FirstOrNone<TSource>(this IEnumerable<TSource> source)
Option<TSource> FirstOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
```

<picture>
    <picture>
      <source srcset="first-or-none-dark.svg" media="(prefers-color-scheme: dark)">
      <img src="first-or-none.svg" alt="A marble diagram showing the FirstOrNone operation">
    </picture>
</picture>

`FirstOrNone` stops enumerating as soon as it has found an element, so it is safe to call on an infinite sequence
as long as a matching element exists.

### Example

```cs
var numbers = Sequence.Return(3, 1, 4, 1, 5);

numbers.FirstOrNone();              // Some(3)
numbers.FirstOrNone(n => n > 3);    // Some(4)
numbers.FirstOrNone(n => n > 10);   // None
Enumerable.Empty<int>().FirstOrNone(); // None
```

A typical use is to replace a `FirstOrDefault` followed by a null check with a chain that keeps the context:

```cs
Option<string> ownerName = accounts
    .FirstOrNone(account => account.Id == id)
    .Select(account => account.Owner.Name);
```
