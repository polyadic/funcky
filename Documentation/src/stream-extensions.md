# Stream Extensions

`System.IO.Stream` is an abstraction over many kinds of streams, and not every stream supports every
operation. A `NetworkStream` cannot seek, so its `Length` and `Position` throw a `NotSupportedException`; a
`MemoryStream` cannot time out, so `ReadTimeout` throws an `InvalidOperationException`. The BCL's answer is a
set of `Can…` flags you are expected to check before touching the property.

Funcky folds the flag and the property into one `Option`-returning call:

| Property or method                  | Funcky                  | `None` when                       |
|-------------------------------------|-------------------------|-----------------------------------|
| `Length`                            | `GetLengthOrNone()`     | `CanSeek` is false                |
| `Position`                          | `GetPositionOrNone()`   | `CanSeek` is false                |
| `ReadTimeout`                       | `GetReadTimeoutOrNone()` | `CanTimeout` is false            |
| `WriteTimeout`                      | `GetWriteTimeoutOrNone()` | `CanTimeout` is false           |
| `ReadByte()`, which returns `-1`    | `ReadByteOrNone()`      | the end of the stream is reached  |

```cs
Option<long> GetLengthOrNone(this Stream stream)
Option<long> GetPositionOrNone(this Stream stream)
Option<int> GetReadTimeoutOrNone(this Stream stream)
Option<int> GetWriteTimeoutOrNone(this Stream stream)
Option<byte> ReadByteOrNone(this Stream stream)
```

The first four are implemented by catching the specific exception the BCL documents for the unsupported case.
Other exceptions, such as `ObjectDisposedException` on a closed stream or an `IOException`, are not caught and
still propagate.

`ReadByteOrNone` removes the `-1` sentinel from `ReadByte`, which returns an `int` only so that it can
signal the end of the stream. The result is a real `byte`.

### Example

```cs
string DescribeProgress(Stream stream)
    => (from position in stream.GetPositionOrNone()
        from length in stream.GetLengthOrNone()
        select $"{position} of {length} bytes")
        .GetOrElse("position unknown");
```

```cs
IEnumerable<byte> Bytes(Stream stream)
    => Sequence.Successors(stream.ReadByteOrNone(), _ => stream.ReadByteOrNone());
```

See [The TryVerb-pattern](./try-pattern.md) for the same idea across the rest of the BCL.
