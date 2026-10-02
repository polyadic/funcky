## GetNonEnumeratedCountOrNone

Returns the number of elements if it can be determined without enumerating the sequence, otherwise `None`.
Available on .NET 6 and later.

```cs
Option<int> GetNonEnumeratedCountOrNone<TSource>(this IEnumerable<TSource> source)
```

This wraps the BCL's `TryGetNonEnumeratedCount`, which succeeds for collections that know their size, such as
arrays, `List<T>` and `ICollection<T>`, and for some LINQ operators over them. It fails for anything that would
need to be iterated, for example the result of a `Where`.

Use it when you want to size a buffer or show a count up front but must not pay for a full enumeration, and
have a fallback for when the count is unknown:

```cs
string Describe<T>(IEnumerable<T> items)
    => items.GetNonEnumeratedCountOrNone()
        .Select(count => $"{count} items")
        .GetOrElse("some items");
```
