using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Funcky.Monads;
using static Funcky.Functional;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// Returns the first element of a sequence as an <see cref="Option{T}" />, or a <see cref="Option{T}.None" /> value if the sequence contains no elements.
        /// </summary>
        /// <typeparam name="TSource">the inner type of the enumerable.</typeparam>
        [Pure]
        public static Option<TSource> FirstOrNone<TSource>(this IEnumerable<TSource> source)
            where TSource : notnull
            => source.FirstOrNone(True);

        /// <summary>
        /// Returns the first element of the sequence as an <see cref="Option{T}" /> that satisfies a condition or a <see cref="Option{T}.None" /> value if no such element is found.
        /// </summary>
        /// <typeparam name="TSource">the inner type of the enumerable.</typeparam>
        [Pure]
        public static Option<TSource> FirstOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
            where TSource : notnull
            => source
                .Where(predicate)
                .Select(Option.Some)
                .FirstOrDefault();
    }
}
