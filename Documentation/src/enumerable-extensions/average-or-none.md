## AverageOrNone

Returns the arithmetic mean of a sequence of numbers, or `None` if the sequence is empty.

```cs
Option<double> AverageOrNone(this IEnumerable<int> source)
Option<double> AverageOrNone(this IEnumerable<long> source)
Option<float> AverageOrNone(this IEnumerable<float> source)
Option<double> AverageOrNone(this IEnumerable<double> source)
Option<decimal> AverageOrNone(this IEnumerable<decimal> source)
```

Each of these also exists for a sequence of `Option<T>` of the same numeric type, and with a selector
(`Func<TSource, int>`, `Func<TSource, Option<int>>`, and so on), matching the overload set of the BCL's `Average`.

<picture>
    <picture>
      <source srcset="average-or-none-dark.svg" media="(prefers-color-scheme: dark)">
      <img src="average-or-none.svg" alt="A marble diagram showing the AverageOrNone operation">
    </picture>
</picture>

The BCL's `Average` throws an `InvalidOperationException` on an empty sequence of non-nullable numbers, and
returns `null` on an empty sequence of nullable numbers. `AverageOrNone` returns `None` in both cases.
`None` elements are skipped and do not count towards the divisor, exactly like `null` elements in the BCL.

The result type follows the BCL: `int` and `long` average to `double`, the floating point types to themselves.

### Example

```cs
Sequence.Return(3, 1, 4, 1, 6).AverageOrNone();    // Some(3.0)
Enumerable.Empty<int>().AverageOrNone();           // None

var temperatures = Sequence.Return(Option.Some(20.0), Option<double>.None, Option.Some(22.0));
temperatures.AverageOrNone();                      // Some(21.0)

orders.AverageOrNone(order => order.Total);        // Option<decimal>
```
