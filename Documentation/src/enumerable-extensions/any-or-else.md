## AnyOrElse

Returns the sequence itself if it has at least one element, otherwise a fallback sequence.

```cs
IEnumerable<TSource> AnyOrElse<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> fallback)
IEnumerable<TSource> AnyOrElse<TSource>(this IEnumerable<TSource> source, Func<IEnumerable<TSource>> fallback)
```

<picture>
    <picture>
      <source srcset="any-or-else-dark.svg" media="(prefers-color-scheme: dark)">
      <img src="any-or-else.svg" alt="A marble diagram showing the AnyOrElse operation">
    </picture>
</picture>

This is `OrElse` from `Option<T>` for sequences, with "empty" playing the role of `None`. It replaces the
`items.Any() ? items : fallback` idiom, which enumerates `items` twice and breaks on a one-shot iterator.
`AnyOrElse` enumerates the source exactly once: it yields the elements as they come and only consults the
fallback once the source has finished without producing anything. The `Func` overload defers building the
fallback until it is actually needed.

`DefaultIfEmpty` from LINQ is the special case with a one-element fallback.

### Example

```cs
var recipients = explicitRecipients.AnyOrElse(() => LoadDefaultRecipients());

var menu = todaysSpecials.AnyOrElse(standardMenu);
```
