using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// Interleaves the elements of multiple sequences by consuming the heads of each subsequence in the same order as the given subsequences. This repeats until all the sequences are completley consumed.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the source sequences.</typeparam>
        /// <param name="sequence">first sequence.</param>
        /// <param name="otherSequences">other sequences.</param>
        /// <returns>one sequences with all the elements interleaved.</returns>
        [Pure]
        public static IEnumerable<TSource> Interleave<TSource>(this IEnumerable<TSource> sequence, params IEnumerable<TSource>[] otherSequences)
            => Interleave(new[] { sequence }.Concat(otherSequences));

        /// <summary>
        /// Interleaves the elements of a sequence of sequences by consuming the heads of each subsequence in the same order as the given subsequences. This repeats until all the sequences are completley consumed.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the source sequences.</typeparam>
        /// <param name="source">the source sequences.</param>
        /// <returns>one sequences with all the elements interleaved.</returns>
        [Pure]
        public static IEnumerable<TSource> Interleave<TSource>(this IEnumerable<IEnumerable<TSource>> source)
        {
            var sourceEnumerators = source.Select(s => s.GetEnumerator()).ToList();

            var enumerators = sourceEnumerators;
            while (enumerators.Count > 0)
            {
                enumerators = enumerators.Where(e => e.MoveNext()).ToList();
                foreach (var enumerator in enumerators)
                {
                    yield return enumerator.Current;
                }
            }

            sourceEnumerators.ForEach(enumerator => enumerator.Dispose());
        }
    }
}
