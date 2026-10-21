## JoinToString

`string.Join` as an extension method, so it can be the last step of a pipeline.

```cs
string JoinToString<TSource>(this IEnumerable<TSource> source, char separator)
string JoinToString<TSource>(this IEnumerable<TSource> source, string separator)
string JoinToString(this IEnumerable<string?> source, string separator)
```

Each element is converted with `ToString`, and the separator goes between the elements, never at the ends. An
empty sequence produces an empty string. The method is eager, as it must be.

`string.Join(", ", items.Select(...))` reads inside out: the last operation comes first. `items.Select(...).JoinToString(", ")`
reads in the order things happen.

An empty separator is concatenation, and [`ConcatToString`](#concattostring) says so; [λ1004](../analyzer-rules/λ1004.md)
points this out.

### Example

```cs
var names = Sequence.Return("Ada", "Grace", "Linus");

names.JoinToString(", ");
// "Ada, Grace, Linus"

Enumerable.Range(1, 5).JoinToString('-');
// "1-2-3-4-5"

var csvLine = record.Fields.Select(Escape).JoinToString(',');
```
