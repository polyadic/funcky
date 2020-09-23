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