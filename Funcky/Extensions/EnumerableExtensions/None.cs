namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    /// <summary>
    /// Determines whether no element exists or satisfies a condition.
    /// </summary>
    [Pure]
    public static bool None<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        => !source.Any(predicate);

    /// <summary>
    /// Determines whether a sequence contains no elements.
    /// </summary>
    [Pure]
    public static bool None<TSource>(this IEnumerable<TSource> source)
        => !source.Any();
}
