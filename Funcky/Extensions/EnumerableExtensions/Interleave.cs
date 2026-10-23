using System.Collections.Immutable;
using Funcky.Internal.Validators;

namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    /// <param name="source">the source sequences.</param>
    /// <typeparam name="TSource">The type of the elements in the source sequences.</typeparam>
    extension<TSource>(IEnumerable<IEnumerable<TSource>> source)
    {
        /// <summary>
        /// Interleaves the elements of a sequence of sequences by consuming the heads of each subsequence in the same order as the given subsequences. This repeats until all the sequences are completely consumed.
        /// </summary>
        /// <returns>one sequences with all the elements interleaved.</returns>
        [Pure]
        public IEnumerable<TSource> Interleave()
            => InterleaveIterator(ArgumentValidator.NotNull(source));
    }

    /// <param name="source">first sequence.</param>
    /// <typeparam name="TSource">The type of the elements in the source sequences.</typeparam>
    extension<TSource>(IEnumerable<TSource> source)
    {
        /// <summary>
        /// Interleaves the elements of multiple sequences by consuming the heads of each subsequence in the same order as the given subsequences. This repeats until all the sequences are completely consumed.
        /// </summary>
        /// <param name="otherSources">other sequences.</param>
        /// <returns>one sequences with all the elements interleaved.</returns>
        [Pure]
        public IEnumerable<TSource> Interleave(params IEnumerable<TSource>[] otherSources)
            => InterleaveIterator([ArgumentValidator.NotNull(source), .. ArgumentValidator.NotNull(otherSources)]);
    }

    private static IEnumerable<TSource> InterleaveIterator<TSource>(IEnumerable<IEnumerable<TSource>> source)
    {
        // The enumerators are acquired inside the try, so the ones acquired before a failing GetEnumerator are disposed.
        var enumerators = new List<IEnumerator<TSource>>();

        try
        {
            foreach (var sequence in source)
            {
                enumerators.Add(sequence.GetEnumerator());
            }

            foreach (var element in InterleaveEnumerator(enumerators.ToImmutableList()))
            {
                yield return element;
            }
        }
        finally
        {
            enumerators.ForEach(enumerator => enumerator.Dispose());
        }
    }

    private static IEnumerable<TSource> InterleaveEnumerator<TSource>(ImmutableList<IEnumerator<TSource>> enumerators)
    {
        while (!enumerators.IsEmpty)
        {
            enumerators = enumerators.RemoveAll(HasMoreElements);
            foreach (var enumerator in enumerators)
            {
                yield return enumerator.Current;
            }
        }
    }
}
