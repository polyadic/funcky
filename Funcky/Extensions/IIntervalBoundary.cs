#if GENERIC_MATH
using System.Numerics;

namespace Funcky.Extensions;

/// <summary>Marks whether an interval boundary includes or excludes its value. See <see cref="Including"/> and <see cref="Excluding"/>.</summary>
public interface IIntervalBoundary
{
    /// <summary>Returns <see langword="true"/> when <paramref name="number"/> is not below <paramref name="boundary"/>, when the boundary is seen as a lower bound.</summary>
    [Pure]
    static abstract bool IsLowerBoundOf<T>(T boundary, T number)
        where T : IComparisonOperators<T, T, bool>;

    /// <summary>Returns <see langword="true"/> when <paramref name="number"/> is not above <paramref name="boundary"/>, when the boundary is seen as an upper bound.</summary>
    [Pure]
    static abstract bool IsUpperBoundOf<T>(T boundary, T number)
        where T : IComparisonOperators<T, T, bool>;
}
#endif
