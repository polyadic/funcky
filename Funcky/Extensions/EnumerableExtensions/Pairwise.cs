using System.Diagnostics.Contracts;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// Returns a sequence of pair-wise tuples from the underlying sequence.
        /// </summary>
        /// <param name="source">The source sequence.</param>
        /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
        /// <returns>Returns a sequence of ValueTuple-pairs.</returns>
        [Pure]
        public static IEnumerable<(TSource Front, TSource Back)> Pairwise<TSource>(this IEnumerable<TSource> source)
            => Pairwise(source, ValueTuple.Create);

        /// <summary>
        /// Applies a function to the element and its successor pairwise from the underlying sequence and returns a new sequence with these results.
        /// I.e. the resulting sequence has one element less then source sequence.
        /// </summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="resultSelector">A function of type (TSource, TSource) -> TResult which creates the results from the pairs.</param>
        /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the result Sequence.</typeparam>
        /// <returns>Returns a sequence of ValueTuple-pairs.</returns>
        [Pure]
        public static IEnumerable<TResult> Pairwise<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TSource, TResult> resultSelector)
        {
            using var enumerator = source.GetEnumerator();

            if (!enumerator.MoveNext())
            {
                yield break;
            }

            for (var previous = enumerator.Current; enumerator.MoveNext(); previous = enumerator.Current)
            {
                yield return resultSelector(previous, enumerator.Current);
            }
        }
    }
}
