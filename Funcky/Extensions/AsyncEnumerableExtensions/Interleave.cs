#if INTEGRATED_ASYNC
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <summary>
    /// Interleaves the elements of multiple sequences by consuming the heads of each subsequence in the same order as the given subsequences. This repeats until all the sequences are completely consumed.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the source sequences.</typeparam>
    /// <param name="source">first sequence.</param>
    /// <param name="otherSources">other sequences.</param>
    /// <returns>one sequences with all the elements interleaved.</returns>
    [Pure]
    public static IAsyncEnumerable<TSource> Interleave<TSource>(this IAsyncEnumerable<TSource> source, params IAsyncEnumerable<TSource>[] otherSources)
        => ImmutableList.Create(source).AddRange(otherSources).Interleave();

    /// <summary>
    /// Interleaves the elements of a sequence of sequences by consuming the heads of each subsequence in the same order as the given subsequences. This repeats until all the sequences are completely consumed.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the source sequences.</typeparam>
    /// <param name="source">the source sequences.</param>
    /// <returns>one sequences with all the elements interleaved.</returns>
    [Pure]
    public static IAsyncEnumerable<TSource> Interleave<TSource>(this IEnumerable<IAsyncEnumerable<TSource>> source) => InterleaveInternal(source);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static async IAsyncEnumerable<TSource> InterleaveInternal<TSource>(
        IEnumerable<IAsyncEnumerable<TSource>> source,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var enumerators = GetInterleaveEnumerators(source, cancellationToken);

        try
        {
            await foreach (var element in InterleaveEnumeratorAsync(enumerators, cancellationToken).ConfigureAwait(false))
            {
                yield return element;
            }
        }
        finally
        {
            await foreach (var enumerator in enumerators.ToAsyncEnumerable().WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                await DisposeEnumerator(enumerator).ConfigureAwait(false);
            }
        }
    }

#pragma warning disable IDISP007 // The entire point of this method is to dispose.
    private static async Task DisposeEnumerator<T>(IAsyncEnumerator<T> enumerator) => await enumerator.DisposeAsync().ConfigureAwait(false);
#pragma warning restore IDISP007

    private static ImmutableList<IAsyncEnumerator<TSource>> GetInterleaveEnumerators<TSource>(
        IEnumerable<IAsyncEnumerable<TSource>> source,
        CancellationToken cancellationToken)
        => source.Select(s => s.GetAsyncEnumerator(cancellationToken)).ToImmutableList();

    private static async IAsyncEnumerable<TSource> InterleaveEnumeratorAsync<TSource>(ImmutableList<IAsyncEnumerator<TSource>> enumerators, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (!enumerators.IsEmpty)
        {
            enumerators = enumerators.RemoveRange(await enumerators.ToAsyncEnumerable().Where(async (f, _) => await HasMoreElements(f).ConfigureAwait(false)).ToListAsync(cancellationToken).ConfigureAwait(false));
            foreach (var enumerator in enumerators)
            {
                yield return enumerator.Current;
            }
        }
    }
}
#endif
