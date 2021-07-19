using System.Diagnostics.Contracts;
using Funcky.Monads;
using static Funcky.Functional;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// Returns the maximum value in a sequence of generic values compared by a <see cref="Comparer{T}"/>. If the sequence is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of generic values of TSource to determine the maximum value of.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<TSource> MaxOrNone<TSource>(this IEnumerable<TSource> source)
            where TSource : notnull
            => source.MaxOrNone(Identity);

        /// <summary>
        /// Returns the maximum value in a sequence of optional generic values compared by a <see cref="Comparer{T}"/>. If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of optional generic values of TSource to determine the maximum value of.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<TSource> MaxOrNone<TSource>(this IEnumerable<Option<TSource>> source)
            where TSource : notnull
            => source.WhereSelect(Identity).MaxOrNone(Identity);

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the maximum from the generic values compared by a <see cref="Comparer{T}"/>. If the transformed sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of generic values of TSource to determine the maximum value of.</param>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<TResult> MaxOrNone<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
            where TResult : notnull
            => source.Select(selector).Aggregate(Option<TResult>.None(), AggregateMax);

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the maximum from the optional generic values compared by a <see cref="Comparer{T}"/>. If the transformed sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of generic values of TSource to determine the maximum value of.</param>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public static Option<TResult> MaxOrNone<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, Option<TResult>> selector)
            where TResult : notnull
            => source.WhereSelect(selector).MaxOrNone(Identity);

        private static Option<TResult> AggregateMax<TResult>(Option<TResult> min, TResult current)
            where TResult : notnull
            => min.Match(current, m => Max(m, current));

        private static TSource Max<TSource>(TSource left, TSource right)
            => Comparer<TSource>.Default.Compare(left, right) > 0 ? left : right;
    }
}
