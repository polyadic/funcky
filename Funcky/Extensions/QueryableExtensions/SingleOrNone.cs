using System.Linq.Expressions;

namespace Funcky.Extensions;

public static partial class QueryableExtensions
{
    /// <summary>
    /// Returns the only element of a sequence as an <see cref="Option{T}" />, or a <see cref="Option{T}.None" /> value if the sequence is empty; this method throws an exception if there is more than one element in the sequence.
    /// </summary>
    /// <typeparam name="TSource">the inner type of the queryable.</typeparam>
    [Pure]
    public static Option<TSource> SingleOrNone<TSource>(this IQueryable<TSource> source)
        where TSource : notnull
        => source
            .Select(x => Option.Some(x))
            .SingleOrDefault();

    /// <summary>
    /// Returns the only element of a sequence that satisfies a specified condition as an <see cref="Option{T}" /> or a <see cref="Option{T}.None" /> value if no such element exists; this method throws an exception if more than one element satisfies the condition.
    /// </summary>
    /// <typeparam name="TSource">the inner type of the queryable.</typeparam>
    [Pure]
    public static Option<TSource> SingleOrNone<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        where TSource : notnull
        => source
            .Where(predicate)
            .Select(x => Option.Some(x))
            .SingleOrDefault();
}
