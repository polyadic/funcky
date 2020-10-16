using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using Funcky.Monads;

namespace Funcky.Extensions
{
    public static partial class QueryableExtensions
    {
        /// <summary>
        /// Returns the last element of a sequence as an <see cref="Option{T}" />, or a <see cref="Option{T}.None" /> value if the sequence contains no elements.
        /// </summary>
        /// <typeparam name="TSource">the inner type of the queryable.</typeparam>
        [Pure]
        public static Option<TSource> LastOrNone<TSource>(this IQueryable<TSource> source)
            where TSource : notnull
            => source
                .Select(x => Option.Some(x))
                .LastOrDefault();

        /// <summary>
        /// Returns the last element of a sequence that satisfies a condition as an <see cref="Option{T}" />  or a <see cref="Option{T}.None" /> value if no such element is found.
        /// </summary>
        /// <typeparam name="TSource">the inner type of the queryable.</typeparam>
        [Pure]
        public static Option<TSource> LastOrNone<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
            where TSource : notnull
            => source
                .Where(predicate)
                .Select(x => Option.Some(x))
                .LastOrDefault();
    }
}
