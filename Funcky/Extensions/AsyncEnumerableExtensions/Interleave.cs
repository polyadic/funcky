#if ASYNC_SUPPORTED
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Funcky.Internal.Validators;

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
        => ImmutableList.Create(ArgumentValidator.NotNull(source)).AddRange(ArgumentValidator.NotNull(otherSources)).Interleave();

    /// <summary>
    /// Interleaves the elements of a sequence of sequences by consuming the heads of each subsequence in the same order as the given subsequences. This repeats until all the sequences are completely consumed.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the source sequences.</typeparam>
    /// <param name="source">the source sequences.</param>
    /// <returns>one sequences with all the elements interleaved.</returns>
    [Pure]
    public static IAsyncEnumerable<TSource> Interleave<TSource>(this IEnumerable<IAsyncEnumerable<TSource>> source) => InterleaveInternal(ArgumentValidator.NotNull(source));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static async IAsyncEnumerable<TSource> InterleaveInternal<TSource>(
        IEnumerable<IAsyncEnumerable<TSource>> source,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // The enumerators are acquired inside the try, so the ones acquired before a failing GetAsyncEnumerator are disposed.
        var enumerators = new List<IAsyncEnumerator<TSource>>();

        try
        {
            foreach (var sequence in source)
            {
                enumerators.Add(sequence.GetAsyncEnumerator(cancellationToken));
            }

            await foreach (var element in InterleaveEnumeratorAsync(enumerators.ToImmutableList(), cancellationToken).ConfigureAwait(false))
            {
                yield return element;
            }
        }
        finally
        {
            foreach (var enumerator in enumerators)
            {
                await DisposeEnumerator(enumerator).ConfigureAwait(false);
            }
        }
    }

#pragma warning disable IDISP007 // The entire point of this method is to dispose.
    private static async Task DisposeEnumerator<T>(IAsyncEnumerator<T> enumerator) => await enumerator.DisposeAsync().ConfigureAwait(false);
#pragma warning restore IDISP007

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
