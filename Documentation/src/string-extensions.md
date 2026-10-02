# String Extensions

## IndexOf

The classical `IndexOf` methods provide a special form of error handling by returning `-1` when nothing is found.
This is very cumbersome and a potential footgun, since you're not forced to check the return value.

Funcky offers extension methods on `string` for each overload of `IndexOf`, `IndexOfAny`, `LastIndexOf`, and `LastIndexOfAny`.
The extension methods follow the simple convention of being suffixed with `OrNone`.


```csharp
Option<string> ParseKey(string input)
    => input.IndexOfOrNone('[')
         .Select(startIndex => ParseKeyWithMultipleParts(input, startIndex))
         .GetOrElse(() => ParseRegularKey(input));
```
*Example usage of `IndexOfOrNone`*
## Chunk and SlidingWindow

The sequence operations [`Chunk`](./enumerable-extensions/grouping-and-windowing.md#chunk) and
[`SlidingWindow`](./enumerable-extensions/grouping-and-windowing.md#slidingwindow) exist on `string` too,
producing strings instead of lists of characters:

```cs
IEnumerable<string> Chunk(this string source, int size)
IEnumerable<string> SlidingWindow(this string source, int width)
```

```cs
"abcdefg".Chunk(3);          // ["abc", "def", "g"]
"abcde".SlidingWindow(3);    // ["abc", "bcd", "cde"]
```

The rules are the same as for sequences: the last chunk may be shorter, only full windows are produced, and a
size or width of zero or less throws an `ArgumentOutOfRangeException`.

## SplitLazy

`string.Split` allocates an array with all parts up front. `SplitLazy` produces the same parts one at a time,
which matters when the string is large and you only need the first few parts, or when the parts are consumed
by a streaming pipeline anyway.

```cs
IEnumerable<string> SplitLazy(this string text, char separator)
IEnumerable<string> SplitLazy(this string text, params char[] separators)
IEnumerable<string> SplitLazy(this string text, string separator)
IEnumerable<string> SplitLazy(this string text, params string[] separators)
```

```cs
var firstField = line.SplitLazy(',').FirstOrNone();
```

## SplitLines

Splits text into lines, treating `\n`, `\r` and `\r\n` each as a line break, lazily.

```cs
IEnumerable<string> SplitLines(this string text)
```

A trailing line break does not produce an empty last line, and the empty string has no lines.

```cs
"first\r\nsecond\nthird\n".SplitLines();   // ["first", "second", "third"]
```
