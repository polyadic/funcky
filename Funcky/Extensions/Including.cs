#if GENERIC_MATH
using System.Numerics;

namespace Funcky.Extensions;

/// <summary>An interval boundary which includes its value.</summary>
public readonly struct Including : IIntervalBoundary
{
    [Pure]
    public static bool IsLowerBoundOf<T>(T boundary, T number)
        where T : IComparisonOperators<T, T, bool>
        => boundary <= number;

    [Pure]
    public static bool IsUpperBoundOf<T>(T boundary, T number)
        where T : IComparisonOperators<T, T, bool>
        => number <= boundary;
}
#endif
