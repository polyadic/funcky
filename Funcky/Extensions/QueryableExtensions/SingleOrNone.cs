using System.Linq.Expressions;

namespace Funcky.Extensions;

public static partial class QueryableExtensions
{
    /// <typeparam name="TSource">the inner type of the queryable.</typeparam>
    extension<TSource>(IQueryable<TSource> source)
        where TSource : notnull
    {
        /// <summary>
        /// Returns the only element of a sequence as an <see cref="Option{T}" />, or a <see cref="Option{T}.None" /> value if the sequence is empty; this method throws an exception if there is more than one element in the sequence.
        /// </summary>
        [Pure]
#if NET7_0
        [System.Diagnostics.CodeAnalysis.RequiresDynamicCode(InMemoryQueryableExtensionMethodsRequiresDynamicCode)]
#endif
        public Option<TSource> SingleOrNone()
            => source
                .Select(x => Option.Some(x))
                .SingleOrDefault();

        /// <summary>
        /// Returns the only element of a sequence that satisfies a specified condition as an <see cref="Option{T}" /> or a <see cref="Option{T}.None" /> value if no such element exists; this method throws an exception if more than one element satisfies the condition.
        /// </summary>
        [Pure]
#if NET7_0
        [System.Diagnostics.CodeAnalysis.RequiresDynamicCode(InMemoryQueryableExtensionMethodsRequiresDynamicCode)]
#endif
        public Option<TSource> SingleOrNone(Expression<Func<TSource, bool>> predicate)
            => source
                .Where(predicate)
                .Select(x => Option.Some(x))
                .SingleOrDefault();
    }
}
