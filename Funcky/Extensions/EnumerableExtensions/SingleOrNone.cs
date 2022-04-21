namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    /// <summary>
    /// Returns the only element of a sequence as an <see cref="Option{T}" />, or a <see cref="Option{T}.None" /> value if the sequence is empty; this method throws an exception if there is more than one element in the sequence.
    /// </summary>
    /// <typeparam name="TSource">the inner type of the enumerable.</typeparam>
    [Pure]
    public static Option<TSource> SingleOrNone<TSource>(this IEnumerable<TSource> source)
        where TSource : notnull
        => source.SingleOrNone(True);

    /// <summary>
    /// Returns the only element of a sequence that satisfies a specified condition as an <see cref="Option{T}" /> or a <see cref="Option{T}.None" /> value if no such element exists; this method throws an exception if more than one element satisfies the condition.
    /// </summary>
    /// <typeparam name="TSource">the inner type of the enumerable.</typeparam>
    [Pure]
    public static Option<TSource> SingleOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        where TSource : notnull
        => source
            .Where(predicate)
            .Select(Option.Some)
            .SingleOrDefault();
}
