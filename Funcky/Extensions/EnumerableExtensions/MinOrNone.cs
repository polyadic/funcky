namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// Returns the minimum value in a sequence of generic values compared by a <see cref="Comparer{T}"/>. If the sequence is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of generic values of <typeparamref name="TSource"/> to determine the minimum value of.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<TSource> MinOrNone<TSource>(this IEnumerable<TSource> source)
            where TSource : notnull
            => source.MinOrNone(Identity);

        /// <summary>
        /// Returns the minimum value in a sequence of optional generic values compared by a <see cref="Comparer{T}"/>. If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <param name="source">A sequence of optional generic values of <typeparamref name="TSource"/> to determine the minimum value of.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<TSource> MinOrNone<TSource>(this IEnumerable<Option<TSource>> source)
            where TSource : notnull
            => source.WhereSelect().MinOrNone(Identity);

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum from the generic values compared by a <see cref="Comparer{T}"/>. If the transformed sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of generic values of <typeparamref name="TSource"/> to determine the minimum value of.</param>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<TResult> MinOrNone<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
            where TResult : notnull
            => source.Select(selector).Aggregate(Option<TResult>.None, AggregateMin);

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum from the optional generic values compared by a <see cref="Comparer{T}"/>. If the transformed sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of generic values of <typeparamref name="TSource"/> to determine the minimum value of.</param>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The minimum value in the sequence or None.</returns>
        [Pure]
        public static Option<TResult> MinOrNone<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, Option<TResult>> selector)
            where TResult : notnull
            => source.WhereSelect(selector).MinOrNone(Identity);

        private static Option<TResult> AggregateMin<TResult>(Option<TResult> min, TResult current)
            where TResult : notnull
            => min.Match(none: current, some: m => Min(m, current));

        // For floats this defines a total order where NaN comes before negative infinity
        private static TSource Min<TSource>(TSource left, TSource right)
            => Comparer<TSource>.Default.Compare(left, right) < 0 ? left : right;
    }
}
