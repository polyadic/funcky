namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    /// <summary>
    /// Returns the last element of a sequence as an <see cref="Option{T}" />, or a <see cref="Option{T}.None" /> value if the sequence contains no elements.
    /// </summary>
    /// <typeparam name="TSource">the inner type of the enumerable.</typeparam>
    [Pure]
    public static Option<TSource> LastOrNone<TSource>(this IEnumerable<TSource> source)
        where TSource : notnull
        => source.LastOrNone(True);

    /// <summary>
    /// Returns the last element of a sequence that satisfies a condition as an <see cref="Option{T}" />  or a <see cref="Option{T}.None" /> value if no such element is found.
    /// </summary>
    /// <typeparam name="TSource">the inner type of the enumerable.</typeparam>
    [Pure]
    public static Option<TSource> LastOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        where TSource : notnull
        => source
            .Where(predicate)
            .Select(Option.Some)
            .LastOrDefault();
}
