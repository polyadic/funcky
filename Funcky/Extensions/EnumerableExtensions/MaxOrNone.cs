using Funcky.Internal.Aggregators;

namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    /// <param name="source">A sequence of generic values of TSource to determine the maximum value of.</param>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    extension<TSource>(IEnumerable<TSource> source)
        where TSource : notnull
    {
        /// <summary>
        /// Returns the maximum value in a sequence of generic values compared by a <see cref="Comparer{T}"/>. If the sequence is empty it returns None.
        /// </summary>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public Option<TSource> MaxOrNone()
            => source.MaxOrNone(Identity);
    }

    /// <param name="source">A sequence of optional generic values of TSource to determine the maximum value of.</param>
    extension<TSource>(IEnumerable<Option<TSource>> source)
        where TSource : notnull
    {
        /// <summary>
        /// Returns the maximum value in a sequence of optional generic values compared by a <see cref="Comparer{T}"/>. If the sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public Option<TSource> MaxOrNone()
            => source.WhereSelect().MaxOrNone(Identity);
    }

    /// <param name="source">A sequence of generic values of TSource to determine the maximum value of.</param>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    extension<TSource>(IEnumerable<TSource> source)
    {
        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the maximum from the generic values compared by a <see cref="Comparer{T}"/>. If the transformed sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public Option<TResult> MaxOrNone<TResult>(Func<TSource, TResult> selector)
            where TResult : notnull
            => source.Select(selector).Aggregate(Option<TResult>.None, MaxAggregator.Aggregate);

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the maximum from the optional generic values compared by a <see cref="Comparer{T}"/>. If the transformed sequence only consists of none or is empty it returns None.
        /// </summary>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the sequence or None.</returns>
        [Pure]
        public Option<TResult> MaxOrNone<TResult>(Func<TSource, Option<TResult>> selector)
            where TResult : notnull
            => source.WhereSelect(selector).MaxOrNone(Identity);
    }
}
