#pragma warning disable RS0026

using System.Collections.Immutable;

namespace Funcky.Async.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <summary>
    /// Merges two ordered sequences into one and preserves the ordering. The merged sequences has exactly the same number of elements as the inputs combined.
    /// </summary>
    /// <remarks>PRECONDITION: The given sequences must be ordered by the same ordering as the given IComparer.</remarks>
    /// <param name="sequence1">First ordered sequence.</param>
    /// <param name="sequence2">Second ordered sequence.</param>
    /// <param name="comparer">The comparer giving the order condition of the sequences.</param>
    /// <typeparam name="TSource">The type elements in the sequences.</typeparam>
    /// <returns>The merged sequences in the same order as the given sequences.</returns>
    [Pure]
    public static IAsyncEnumerable<TSource> Merge<TSource>(this IAsyncEnumerable<TSource> sequence1, IAsyncEnumerable<TSource> sequence2, Option<IComparer<TSource>> comparer = default)
        => ImmutableList.Create(sequence1, sequence2).Merge(comparer);

    /// <summary>
    /// Merges three ordered sequences into one and preserves the ordering. The merged sequences has exactly the same number of elements as the inputs combined.
    /// </summary>
    /// <remarks>PRECONDITION: The given sequences must be ordered by the same ordering as the given IComparer.</remarks>
    /// <param name="sequence1">First ordered sequence.</param>
    /// <param name="sequence2">Second ordered sequence.</param>
    /// <param name="sequence3">Third ordered sequence.</param>
    /// <param name="comparer">The comparer giving the order condition of the sequences.</param>
    /// <typeparam name="TSource">The type elements in the sequences.</typeparam>
    /// <returns>The merged sequences in the same order as the given sequences.</returns>
    [Pure]
    public static IAsyncEnumerable<TSource> Merge<TSource>(this IAsyncEnumerable<TSource> sequence1, IAsyncEnumerable<TSource> sequence2, IAsyncEnumerable<TSource> sequence3, Option<IComparer<TSource>> comparer = default)
        => ImmutableList.Create(sequence1, sequence2, sequence3).Merge(comparer);

    /// <summary>
    /// Merges three ordered sequences into one and preserves the ordering. The merged sequences has exactly the same number of elements as the inputs combined.
    /// </summary>
    /// <remarks>PRECONDITION: The given sequences must be ordered by the same ordering as the given IComparer.</remarks>
    /// <param name="sequence1">First ordered sequence.</param>
    /// <param name="sequence2">Second ordered sequence.</param>
    /// <param name="sequence3">Third ordered sequence.</param>
    /// <param name="sequence4">Forth ordered sequence.</param>
    /// <param name="comparer">The comparer giving the order condition of the sequences.</param>
    /// <typeparam name="TSource">The type elements in the sequences.</typeparam>
    /// <returns>The merged sequences in the same order as the given sequences.</returns>
    [Pure]
    public static IAsyncEnumerable<TSource> Merge<TSource>(this IAsyncEnumerable<TSource> sequence1, IAsyncEnumerable<TSource> sequence2, IAsyncEnumerable<TSource> sequence3, IAsyncEnumerable<TSource> sequence4, Option<IComparer<TSource>> comparer = default)
        => ImmutableList.Create(sequence1, sequence2, sequence3, sequence4).Merge(comparer);

    /// <summary>
    /// Merges a sequence of ordered sequences into one and preserves the ordering. The merged sequences has exactly the same number of elements as the inputs combined.
    /// </summary>
    /// <remarks>PRECONDITION: The given sequences must be ordered by the same ordering as the given IComparer.</remarks>
    /// <param name="sources">First ordered sequence.</param>
    /// <param name="comparer">The comparer giving the order condition of the sequences.</param>
    /// <typeparam name="TSource">The type elements in the sequences.</typeparam>
    /// <returns>The merged sequences in the same order as the given sequences.</returns>
    [Pure]
    public static async IAsyncEnumerable<TSource> Merge<TSource>(this IEnumerable<IAsyncEnumerable<TSource>> sources, Option<IComparer<TSource>> comparer = default)
    {
        var enumerators = GetMergeEnumerators(sources);

        try
        {
            await foreach (var element in MergeEnumerators(enumerators.RemoveRange(await enumerators.ToAsyncEnumerable().WhereAwait(async f => await HasMoreElements(f).ConfigureAwait(false)).ToListAsync().ConfigureAwait(false)), GetMergeComparer(comparer)))
            {
                yield return element;
            }
        }
        finally
        {
            foreach (var enumerator in enumerators)
            {
                await enumerator.DisposeAsync().ConfigureAwait(false);
            }
        }
    }

    private static ImmutableList<IAsyncEnumerator<TSource>> GetMergeEnumerators<TSource>(IEnumerable<IAsyncEnumerable<TSource>> sources)
        => ImmutableList.Create<IAsyncEnumerator<TSource>>().AddRange(sources.Select(s => s.GetAsyncEnumerator()));

    private static IComparer<TSource> GetMergeComparer<TSource>(Option<IComparer<TSource>> comparer = default)
        => comparer.GetOrElse(Comparer<TSource>.Default);

    private static async Task<bool> HasMoreElements<TSource>(IAsyncEnumerator<TSource> enumerator)
        => !await enumerator.MoveNextAsync().ConfigureAwait(false);

    private static async IAsyncEnumerable<TSource> MergeEnumerators<TSource>(ImmutableList<IAsyncEnumerator<TSource>> enumerators, IComparer<TSource> comparer)
    {
        while (!enumerators.IsEmpty)
        {
            var minimum = FindMinimum(enumerators, comparer);
            yield return minimum.Current;

            enumerators = await RemoveYieldedAsync(minimum, enumerators).ConfigureAwait(false);
        }
    }

    private static async Task<ImmutableList<IAsyncEnumerator<TSource>>> RemoveYieldedAsync<TSource>(IAsyncEnumerator<TSource> minimum, ImmutableList<IAsyncEnumerator<TSource>> enumerators)
        => await minimum.MoveNextAsync().ConfigureAwait(false)
            ? enumerators
            : enumerators.Remove(minimum);

    private static IAsyncEnumerator<TSource> FindMinimum<TSource>(ImmutableList<IAsyncEnumerator<TSource>> enumerators, IComparer<TSource> comparer)
        => enumerators.Aggregate(Minimum(comparer));

    private static Func<IAsyncEnumerator<TSource>, IAsyncEnumerator<TSource>, IAsyncEnumerator<TSource>> Minimum<TSource>(IComparer<TSource> comparer)
        => (enumerator, minimum) => comparer.Compare(minimum.Current, enumerator.Current) <= 0 ? minimum : enumerator;
}
