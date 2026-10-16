## InspectEmpty

Runs an action if the sequence turns out to be empty, and yields the elements unchanged otherwise.

```cs
IEnumerable<TSource> InspectEmpty<TSource>(this IEnumerable<TSource> source, Action inspector)
```

<picture>
    <picture>
      <source srcset="inspect-empty-dark.svg" media="(prefers-color-scheme: dark)">
      <img src="inspect-empty.svg" alt="A marble diagram showing the InspectEmpty operation">
    </picture>
</picture>

This is the counterpart of `Inspect` for the case `Inspect` can never see: a sequence with no elements. Like
`Inspect` it is lazy; the action runs during enumeration, at the point where the first element was requested
and the source reported that there is none. It corresponds to `InspectNone` on `Option<T>`.

### Example

```cs
var results = search(query)
    .InspectEmpty(() => logger.LogInformation("no results for {Query}", query))
    .Select(FormatResult);
```
