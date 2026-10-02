# The TryVerb-pattern

Many operations cannot always produce a value. A string may not be a number, a key may not be in the dictionary,
a queue may be empty. The BCL has grown three different conventions for saying "there is nothing here", and
`Option<T>` replaces all of them. This page explains why, and lists every BCL member for which Funcky provides an
`…OrNone` alternative.

## Three conventions for one problem

**Throwing.** The oldest convention: `int.Parse` throws a `FormatException` when the input is not a number.
The failure is invisible in the signature, expensive at runtime, and easy to forget to handle.

```cs
var number = int.Parse(input); // throws on "abc"
```

**Sentinel values.** `IndexOf` returns `-1`, `FirstOrDefault` returns `null` or `0`, `Stream.ReadByte` returns
`-1`. The failure is encoded as a value of the same type, so nothing forces the caller to check it, and for
`FirstOrDefault` on a `List<int>` you cannot even tell an absent element from a present zero.

```cs
var index = text.IndexOf(':');
var key = text.Substring(0, index); // ArgumentOutOfRangeException when index is -1
```

**The TryVerb-pattern.** `TryParse`, `TryGetValue`, `TryDequeue` return a `bool` and hand the value back through
an `out` parameter. This is the convention the BCL has settled on, and it is better than the other two: the
failure is in the signature, and the compiler nudges you into checking the `bool`.

```cs
if (int.TryParse(input, out var number))
{
    // number is valid here
}
```

It is still a workaround. The method wants to return one of two things, a number or nothing, and C# had no way
to express that in a single return value. So it returns two values and leaves it to a convention that the
`out` parameter is only meaningful when the `bool` is `true`.

## Why it is unnecessary with sum types

A type that holds one of several alternatives is called a sum type. `Option<T>` is the simplest one: `Some(T)` or
`None`. Once you have it, the `TryVerb`-pattern is just `Option<T>` flattened into two variables, and all of its
awkwardness comes from that flattening:

* **The correlation is a convention, not a type.** Nothing stops you from reading `number` when `TryParse`
  returned `false`. Nullable annotations such as `[NotNullWhen(true)]` patch this for reference types but cannot
  express it for `int`. With `Option<int>` there is no `int` to read until you have handled the `None` case.
* **`out` is a statement, not an expression.** You need a variable declared before the call, which means a
  mutable local, a separate `if`, and a block. You cannot use a `Try` method in a LINQ query, chain it, pass it
  as a `Func`, or write it as an expression-bodied member without a helper.
* **It does not compose.** Parsing three numbers from three strings takes three `if`s and three temporaries.
  Three `Option<int>` compose with `SelectMany` into one expression that is `None` if any of them is.
* **It does not work everywhere.** Iterators and `async` methods cannot have `out` parameters, so the pattern
  cannot even be used consistently across the BCL's own asynchronous APIs.

Compare the two versions of the same function:

```cs
// TryVerb-pattern
bool TryGetPort(IReadOnlyDictionary<string, string> settings, out int port)
{
    port = default;
    return settings.TryGetValue("port", out var text)
        && int.TryParse(text, out port)
        && port > 0;
}

// Option<T>
Option<int> GetPort(IReadOnlyDictionary<string, string> settings)
    => settings.GetValueOrNone("port")
        .SelectMany(ParseExtensions.ParseInt32OrNone)
        .Where(port => port > 0);
```

The second one is shorter, has no mutable state, and is itself usable in the next `SelectMany`. The first one
cannot be, because it has an `out` parameter, so every caller starts the `if` dance again.

The transformation from a `Try` method to an `Option` is mechanical:

```cs
Option<int> ParseInt32OrNone(this string candidate)
    => int.TryParse(candidate, out var result)
        ? result
        : Option<int>.None;
```

Funcky applies it once, at the library boundary, for every `Try` member it knows about, so your code never has
to. For the `Parse` family this is done by a source generator that mirrors every overload of the BCL `TryParse`
method, including the ones taking `NumberStyles`, `IFormatProvider`, `ReadOnlySpan<char>` and UTF-8
`ReadOnlySpan<byte>`. If you find a `Try`
method in the BCL that has no `…OrNone` counterpart, that is a bug report we are happy to receive.

## Naming

All methods follow the same convention: the verb of the BCL member, suffixed with `OrNone`. The subject of the
`Try` method becomes the receiver of an extension method, so `int.TryParse(text, out var n)` becomes
`text.ParseInt32OrNone()`, and `dictionary.TryGetValue(key, out var v)` becomes `dictionary.GetValueOrNone(key)`.
Methods that replace a sentinel keep the BCL name, so `FirstOrDefault` becomes `FirstOrNone` and `IndexOf` becomes
`IndexOfOrNone`.

The type parameter of the resulting `Option<T>` is constrained to `notnull`. Where the BCL method can hand back
`null`, for example `FirstOrDefault` on a sequence of nullable references, the `…OrNone` variant requires a
non-nullable element type; use `WhereNotNull` first.

## Parsing

All parse methods are extension methods on `string` (and on `ReadOnlySpan<char>` and `ReadOnlySpan<byte>` where
the BCL offers it) in the
static class `ParseExtensions`. Every overload of the BCL `TryParse` is mirrored, so `"2A".ParseInt32OrNone(NumberStyles.HexNumber, CultureInfo.InvariantCulture)`
works exactly like the corresponding `int.TryParse`.

| BCL                                               | Funcky                              | Available on   |
|---------------------------------------------------|-------------------------------------|----------------|
| `byte.TryParse`                                   | `ParseByteOrNone`                   | all            |
| `sbyte.TryParse`                                  | `ParseSByteOrNone`                  | all            |
| `short.TryParse`                                  | `ParseInt16OrNone`                  | all            |
| `ushort.TryParse`                                 | `ParseUInt16OrNone`                 | all            |
| `int.TryParse`                                    | `ParseInt32OrNone`                  | all            |
| `uint.TryParse`                                   | `ParseUInt32OrNone`                 | all            |
| `long.TryParse`                                   | `ParseInt64OrNone`                  | all            |
| `ulong.TryParse`                                  | `ParseUInt64OrNone`                 | all            |
| `float.TryParse`                                  | `ParseSingleOrNone`                 | all            |
| `double.TryParse`                                 | `ParseDoubleOrNone`                 | all            |
| `decimal.TryParse`                                | `ParseDecimalOrNone`                | all            |
| `BigInteger.TryParse`                             | `ParseBigIntegerOrNone`             | all            |
| `bool.TryParse`                                   | `ParseBooleanOrNone`                | all            |
| `char.TryParse`                                   | `ParseCharOrNone`                   | all            |
| `Enum.TryParse<TEnum>`                            | `ParseEnumOrNone<TEnum>`            | all            |
| `Enum.TryParse(Type, …)`                          | `ParseEnumOrNone(Type)`             | .NET Standard 2.1+ |
| `Guid.TryParse` / `Guid.TryParseExact`            | `ParseGuidOrNone` / `ParseExactGuidOrNone` | all     |
| `DateTime.TryParse` / `TryParseExact`             | `ParseDateTimeOrNone` / `ParseExactDateTimeOrNone` | all |
| `DateTimeOffset.TryParse` / `TryParseExact`       | `ParseDateTimeOffsetOrNone` / `ParseExactDateTimeOffsetOrNone` | all |
| `TimeSpan.TryParse` / `TryParseExact`             | `ParseTimeSpanOrNone` / `ParseExactTimeSpanOrNone` | all |
| `DateOnly.TryParse` / `TryParseExact`             | `ParseDateOnlyOrNone` / `ParseExactDateOnlyOrNone` | .NET 6+ |
| `TimeOnly.TryParse` / `TryParseExact`             | `ParseTimeOnlyOrNone` / `ParseExactTimeOnlyOrNone` | .NET 6+ |
| `Version.TryParse`                                | `ParseVersionOrNone`                | all            |
| `IPAddress.TryParse`                              | `ParseIPAddressOrNone`              | all            |
| `IPEndPoint.TryParse`                             | `ParseIPEndPointOrNone`             | .NET Core 3.0+ |
| `IPNetwork.TryParse`                              | `ParseIPNetworkOrNone`              | .NET 8+        |
| `AssemblyNameInfo.TryParse`                       | `ParseAssemblyNameInfoOrNone`       | .NET 9+        |
| `TypeName.TryParse`                               | `ParseTypeNameOrNone`               | .NET 9+        |
| `INumberBase<T>.TryParse`                         | `ParseNumberOrNone<TNumber>`        | .NET 7+        |
| `IParsable<T>.TryParse` / `ISpanParsable<T>.TryParse` | `ParseOrNone<TParsable>`        | .NET 7+        |
| `IUtf8SpanParsable<T>.TryParse`                   | `ParseOrNone<TParsable>` on `ReadOnlySpan<byte>` | .NET 8+ |

The generic `ParseOrNone<T>` and `ParseNumberOrNone<T>` cover any type implementing the static abstract parsing
interfaces, including your own, so on .NET 7 and later `text.ParseOrNone<Guid>(null)` and
`text.ParseGuidOrNone()` are equivalent.

The `TryParse` methods of the `System.Net.Http.Headers` value types are mirrored as well:

| BCL                                               | Funcky                                            |
|---------------------------------------------------|---------------------------------------------------|
| `AuthenticationHeaderValue.TryParse`              | `ParseAuthenticationHeaderValueOrNone`            |
| `CacheControlHeaderValue.TryParse`                | `ParseCacheControlHeaderValueOrNone`              |
| `ContentDispositionHeaderValue.TryParse`          | `ParseContentDispositionHeaderValueOrNone`        |
| `ContentRangeHeaderValue.TryParse`                | `ParseContentRangeHeaderValueOrNone`              |
| `EntityTagHeaderValue.TryParse`                   | `ParseEntityTagHeaderValueOrNone`                 |
| `MediaTypeHeaderValue.TryParse`                   | `ParseMediaTypeHeaderValueOrNone`                 |
| `MediaTypeWithQualityHeaderValue.TryParse`        | `ParseMediaTypeWithQualityHeaderValueOrNone`      |
| `NameValueHeaderValue.TryParse`                   | `ParseNameValueHeaderValueOrNone`                 |
| `NameValueWithParametersHeaderValue.TryParse`     | `ParseNameValueWithParametersHeaderValueOrNone`   |
| `ProductHeaderValue.TryParse`                     | `ParseProductHeaderValueOrNone`                   |
| `ProductInfoHeaderValue.TryParse`                 | `ParseProductInfoHeaderValueOrNone`               |
| `RangeConditionHeaderValue.TryParse`              | `ParseRangeConditionHeaderValueOrNone`            |
| `RangeHeaderValue.TryParse`                       | `ParseRangeHeaderValueOrNone`                     |
| `RetryConditionHeaderValue.TryParse`              | `ParseRetryConditionHeaderValueOrNone`            |
| `StringWithQualityHeaderValue.TryParse`           | `ParseStringWithQualityHeaderValueOrNone`         |
| `TransferCodingHeaderValue.TryParse`              | `ParseTransferCodingHeaderValueOrNone`            |
| `TransferCodingWithQualityHeaderValue.TryParse`   | `ParseTransferCodingWithQualityHeaderValueOrNone` |
| `ViaHeaderValue.TryParse`                         | `ParseViaHeaderValueOrNone`                       |
| `WarningHeaderValue.TryParse`                     | `ParseWarningHeaderValueOrNone`                   |

## Collections

| BCL                                                     | Funcky                       | Available on   |
|---------------------------------------------------------|------------------------------|----------------|
| `IDictionary<K, V>.TryGetValue`                         | `GetValueOrNone`             | all            |
| `IReadOnlyDictionary<K, V>.TryGetValue`                 | `GetValueOrNone`             | all            |
| `IDictionary<K, V>.Remove(key, out value)`              | `RemoveOrNone`               | .NET Standard 2.1+ |
| `OrderedDictionary<K, V>.IndexOf` (returns `-1`)        | `IndexOfOrNone`              | .NET 9+        |
| `Queue<T>.TryDequeue` / `TryPeek`                       | `DequeueOrNone` / `PeekOrNone` | all ¹        |
| `ConcurrentQueue<T>.TryDequeue` / `TryPeek`             | `DequeueOrNone` / `PeekOrNone` | all          |
| `PriorityQueue<T, P>.TryDequeue` / `TryPeek`            | `DequeueOrNone` / `PeekOrNone`, returning a `(Element, Priority)` tuple | .NET 6+ |
| `IEnumerator<T>.MoveNext` + `Current`                   | `MoveNextOrNone`             | all            |
| `Enumerable.TryGetNonEnumeratedCount`                   | `GetNonEnumeratedCountOrNone` | .NET 6+       |
| `IList<T>.IndexOf` (returns `-1`)                       | `IndexOfOrNone`              | all            |
| `List<T>.FindIndex` / `FindLastIndex` (return `-1`)     | `FindIndexOrNone` / `FindLastIndexOrNone` | all |
| `IImmutableList<T>.IndexOf` / `LastIndexOf` (return `-1`) | `IndexOfOrNone` / `LastIndexOfOrNone` | all  |

¹ `Queue<T>` only gained `TryDequeue` and `TryPeek` in .NET Standard 2.1. On .NET Standard 2.0 Funcky falls back
to catching the `InvalidOperationException` that `Dequeue` and `Peek` throw on an empty queue, so the behaviour is
the same on every target.

## LINQ

These replace the `…OrDefault` family, whose sentinel is `default(T)` and therefore ambiguous for value types.
They are available on `IEnumerable<T>` and `IQueryable<T>`, and as `…OrNoneAsync` on `IAsyncEnumerable<T>`
(in `Funcky` itself on .NET 10, in the `Funcky.Async` package on older targets). See [IEnumerable Extensions](./enumerable-extensions/enumerable-extensions.md).

| BCL                                     | Funcky                          |
|-----------------------------------------|---------------------------------|
| `FirstOrDefault`                        | `FirstOrNone`                   |
| `LastOrDefault`                         | `LastOrNone`                    |
| `SingleOrDefault`                       | `SingleOrNone`                  |
| `ElementAtOrDefault`                    | `ElementAtOrNone`               |
| `Min` / `Max` (return `null` for nullable types, throw otherwise) | `MinOrNone` / `MaxOrNone` |
| `MinBy` / `MaxBy`                       | `MinByOrNone` / `MaxByOrNone`   |
| `Average` (returns `null` for nullable types, throws otherwise) | `AverageOrNone` |

## Strings

The `IndexOf` family returns `-1` when nothing is found. Funcky mirrors every overload, including the ones taking
a `StringComparison` or a start index and count. See [String Extensions](./string-extensions.md).

| BCL                       | Funcky                   |
|---------------------------|--------------------------|
| `string.IndexOf`          | `IndexOfOrNone`          |
| `string.IndexOfAny`       | `IndexOfAnyOrNone`       |
| `string.LastIndexOf`      | `LastIndexOfOrNone`      |
| `string.LastIndexOfAny`   | `LastIndexOfAnyOrNone`   |

## Streams and I/O

`Stream` uses yet another convention: properties that throw when the stream does not support the operation, with a
separate `CanSeek` or `CanTimeout` flag you are expected to check first. The `…OrNone` variants fold the flag into
the result.

| BCL                                                   | Funcky                  | Returns `None` when |
|-------------------------------------------------------|-------------------------|---------------------|
| `Stream.Length`                                       | `GetLengthOrNone`       | `CanSeek` is false  |
| `Stream.Position`                                     | `GetPositionOrNone`     | `CanSeek` is false  |
| `Stream.ReadTimeout`                                  | `GetReadTimeoutOrNone`  | `CanTimeout` is false |
| `Stream.WriteTimeout`                                 | `GetWriteTimeoutOrNone` | `CanTimeout` is false |
| `Stream.ReadByte` (returns `-1`)                      | `ReadByteOrNone`        | at end of stream    |

## Miscellaneous

| BCL                                                   | Funcky               | Available on |
|-------------------------------------------------------|----------------------|--------------|
| `HttpHeaders.TryGetValues`                            | `GetValuesOrNone`    | all          |
| `HttpHeadersNonValidated.TryGetValues`                | `GetValuesOrNone`    | .NET 6+      |
| `JsonSerializerOptions.TryGetTypeInfo`                | `GetTypeInfoOrNone`  | .NET 8+      |

## Everything else: `Option.FromNullable`

Many BCL members simply return `null`, from `Environment.GetEnvironmentVariable` to `Type.GetMethod`. There is no
`…OrNone` variant for each of them, because one function covers them all:

```cs
Option<string> home = Option.FromNullable(Environment.GetEnvironmentVariable("HOME"));
```

`FromNullable` has overloads for nullable reference types and `Nullable<T>`, and is the right tool whenever a
`null` means "absent". The reverse, `ToNullable`, is how an `Option<T>` leaves your code again at an API that
expects `null`; see [Option Monad](./option.md).
