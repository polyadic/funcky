#if ASYNC_SUPPORTED
using Funcky.Internal;

namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <summary>
    /// Returns the elements of the given sequence in random order.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the enumerable.</typeparam>
    /// <remarks>
    /// This method is eager: the source is enumerated once when the returned task is awaited, and the result is an
    /// independent list shuffled in O(n) with a Fisher-Yates shuffle, so every permutation is equally likely. The random
    /// numbers come from a shared generator; pass a <see cref="Random"/> for reproducible results.
    /// </remarks>
    public static async ValueTask<IReadOnlyList<TSource>> ShuffleAsync<TSource>(this IAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default)
        => await source.ShuffleAsync(RandomProvider.Shared, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Returns the elements of the given sequence in random order, drawing the random numbers from <paramref name="random"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the enumerable.</typeparam>
    /// <remarks>
    /// This method is eager: the source is enumerated once when the returned task is awaited, and the result is an
    /// independent list shuffled in O(n) with a Fisher-Yates shuffle, so every permutation is equally likely. The state
    /// of <paramref name="random"/> is advanced; two calls with the same instance return different orders, two calls
    /// with generators seeded alike return the same order.
    /// </remarks>
    public static async ValueTask<IReadOnlyList<TSource>> ShuffleAsync<TSource>(this IAsyncEnumerable<TSource> source, Random random, CancellationToken cancellationToken = default)
    {
        var materialized = await source.ToArrayAsync(cancellationToken).ConfigureAwait(false);
        random.Shuffle(materialized);
        return materialized;
    }
}
#endif
