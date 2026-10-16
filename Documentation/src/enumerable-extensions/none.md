## None

Returns `true` if the sequence is empty, or if no element satisfies the predicate. It is `!Any()`, with the
negation where it is easy to see.

```cs
bool None<TSource>(this IEnumerable<TSource> source)
bool None<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
```

Like `Any`, `None` stops at the first element that decides the answer, so it is safe on infinite sequences as
long as a matching element exists.

`!items.Any(x => x.IsValid)` reads as "not any valid" and is easy to confuse with `items.Any(x => !x.IsValid)`,
which means something else. `items.None(x => x.IsValid)` says what it checks.

### Example

```cs
if (errors.None())
{
    Commit();
}

bool allSettled = payments.None(p => p.IsPending);
```
