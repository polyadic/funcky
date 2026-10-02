## ConcatToString

`string.Concat` as an extension method: all elements converted with `ToString` and glued together with nothing
in between.

```cs
string ConcatToString<TSource>(this IEnumerable<TSource> source)
```

The typical input is a sequence of `char` or of `string`, for example the result of a character-level
transformation that should become a string again.

### Example

```cs
"hello".Reverse().ConcatToString();
// "olleh"

"Hello World".Where(char.IsLetter).ConcatToString();
// "HelloWorld"

var html = elements.Select(RenderElement).ConcatToString();
```
