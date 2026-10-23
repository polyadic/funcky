# Number Extensions

## InRange

Checking whether a number lies within an interval is usually written as two comparisons, and whether each end of the
interval is included is only visible in the choice between `<` and `<=`. `InRange` makes the kind of each boundary
part of the call, so it reads like interval notation:

```cs
bool InRange<TFrom, TTo>(this int number, int from, int to)
    where TFrom : IIntervalBoundary
    where TTo : IIntervalBoundary
```

The type arguments are `Including` or `Excluding`, one for each boundary:

```cs
12.InRange<Including, Excluding>(0, 20);   // true:  0 <= 12 && 12 < 20
20.InRange<Including, Excluding>(0, 20);   // false: the upper boundary is excluded
 0.InRange<Excluding, Excluding>(0, 20);   // false: the lower boundary is excluded
 5.InRange<Including, Including>(5, 5);    // true:  the only interval containing a single value
```

The boundaries may be given in any order; `12.InRange<Including, Excluding>(20, 0)` is the same check as above.

`InRange` exists on every numeric type of the base class library (`int`, `long`, `double`, `decimal`, `Int128`,
`Half`, `BigInteger`, `char`, …). It requires .NET 7 or newer, because the implementation is built on generic math.
A custom type implementing `IComparisonOperators<T, T, bool>` can use the static method directly:

```cs
NumberExtensions.InRange<Money, Including, Excluding>(amount, Money.Zero, limit);
```

The `Including` and `Excluding` types carry no state. They implement `IIntervalBoundary` through static abstract
members, so the comparison is resolved at compile time and the check costs the same as writing it by hand.
