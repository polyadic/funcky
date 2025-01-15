using System.Collections.Immutable;

namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    /// <summary>
    /// Interleaves the elements of multiple sequences by consuming the heads of each subsequence in the same order as the given subsequences. This repeats until all the sequences are completely consumed.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the source sequences.</typeparam>
    /// <param name="source">first sequence.</param>
    /// <param name="otherSources">other sequences.</param>
    /// <returns>one sequences with all the elements interleaved.</returns>
    [Pure]
    public static IEnumerable<TSource> Interleave<TSource>(this IEnumerable<TSource> source, params IEnumerable<TSource>[] otherSources)
        => Interleave([source, .. otherSources]);

    /// <summary>
    /// Interleaves the elements of a sequence of sequences by consuming the heads of each subsequence in the same order as the given subsequences. This repeats until all the sequences are completely consumed.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the source sequences.</typeparam>
    /// <param name="source">the source sequences.</param>
    /// <returns>one sequences with all the elements interleaved.</returns>
    [Pure]
    public static IEnumerable<TSource> Interleave<TSource>(this IEnumerable<IEnumerable<TSource>> source)
    {
        var enumerators = GetInterleaveEnumerators(source);

        try
        {
            foreach (var element in InterleaveEnumerator(enumerators))
            {
                yield return element;
            }
        }
        finally
        {
            enumerators.ForEach(enumerator => enumerator.Dispose());
        }
    }

    private static ImmutableList<IEnumerator<TSource>> GetInterleaveEnumerators<TSource>(IEnumerable<IEnumerable<TSource>> source)
        => source.Select(s => s.GetEnumerator()).ToImmutableList();

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
