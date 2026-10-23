using Funcky.Internal;

namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    /// <param name="source">A sequence of values to determine the maximum value of.</param>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
    extension<TSource>(IEnumerable<TSource> source)
        where TSource : notnull
    {
        /// <summary>Returns the maximum value in a generic sequence according to a specified key selector function.</summary>
        /// <typeparam name="TKey">The type of key to compare elements by.</typeparam>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <returns>The value with the maximum key in the sequence or None.</returns>
        public Option<TSource> MaxByOrNone<TKey>(Func<TSource, TKey> keySelector)
            => MaxByOrNone(source, keySelector, Comparer<TKey>.Default);

        /// <summary>Returns the maximum value in a generic sequence according to a specified key selector function.</summary>
        /// <typeparam name="TKey">The type of key to compare elements by.</typeparam>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <param name="comparer">The <see cref="IComparer{TKey}" /> to compare keys.</param>
        /// <returns>The value with the maximum key in the sequence or None.</returns>
        [Pure]
        public Option<TSource> MaxByOrNone<TKey>(Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
            => ExtremumByOrNone.MaxByOrNone(source, keySelector, comparer);
    }
}
