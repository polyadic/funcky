using System.Linq.Expressions;

namespace Funcky.Extensions;

public static partial class QueryableExtensions
{
    /// <typeparam name="TSource">the inner type of the queryable.</typeparam>
    extension<TSource>(IQueryable<TSource> source)
        where TSource : notnull
    {
        /// <summary>
        /// Returns the first element of a sequence as an <see cref="Option{T}" />, or a <see cref="Option{T}.None" /> value if the sequence contains no elements.
        /// </summary>
        [Pure]
#if NET7_0
        [System.Diagnostics.CodeAnalysis.RequiresDynamicCode(InMemoryQueryableExtensionMethodsRequiresDynamicCode)]
#endif
        public Option<TSource> FirstOrNone()
            => source
                .Select(x => Option.Some(x))
                .FirstOrDefault();

        /// <summary>
        /// Returns the first element of the sequence as an <see cref="Option{T}" /> that satisfies a condition or a <see cref="Option{T}.None" /> value if no such element is found.
        /// </summary>
        [Pure]
#if NET7_0
        [System.Diagnostics.CodeAnalysis.RequiresDynamicCode(InMemoryQueryableExtensionMethodsRequiresDynamicCode)]
#endif
        public Option<TSource> FirstOrNone(Expression<Func<TSource, bool>> predicate)
            => source
                .Where(predicate)
                .Select(x => Option.Some(x))
                .FirstOrDefault();
    }
}
