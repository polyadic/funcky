## SingleOrNone

Returns the only element of a sequence, or `None` if the sequence is empty. With a predicate, returns the only
element that satisfies it.

```cs
Option<TSource> SingleOrNone<TSource>(this IEnumerable<TSource> source)
Option<TSource> SingleOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
```

<picture>
    <picture>
      <source srcset="single-or-none-dark.svg" media="(prefers-color-scheme: dark)">
      <img src="single-or-none.svg" alt="A marble diagram showing the SingleOrNone operation">
    </picture>
</picture>

`SingleOrNone` keeps the contract of `SingleOrDefault`: an empty sequence is a legitimate answer and yields
`None`, but **more than one matching element is a programming error** and throws an `InvalidOperationException`.
If "zero or more than one" should both be `None`, use `Where` and a count, or `FirstOrNone` if you only care
about the first match.

To find out whether there is exactly one element, `SingleOrNone` has to look at the second one, so it enumerates
at most two elements.

### Example

```cs
Sequence.Return(4).SingleOrNone();             // Some(4)
Enumerable.Empty<int>().SingleOrNone();        // None
Sequence.Return(4, 4).SingleOrNone();          // throws InvalidOperationException

var users = Sequence.Return("alice", "bob");
users.SingleOrNone(u => u.StartsWith('a'));    // Some("alice")
users.SingleOrNone(u => u.StartsWith('z'));    // None
```
