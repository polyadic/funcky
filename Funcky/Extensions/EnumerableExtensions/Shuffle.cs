#if !SHUFFLE_EXTENSION
using Funcky.Internal;
#endif

namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
#if !SHUFFLE_EXTENSION
    /// <summary>
    /// Returns the elements of the given sequence in random order.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the enumerable.</typeparam>
    /// <remarks>
    /// This method is eager: the source is enumerated once when it is called, and the result is an independent list
    /// shuffled in O(n) with a Fisher-Yates shuffle, so every permutation is equally likely. The random numbers come
    /// from a shared generator; pass a <see cref="Random"/> for reproducible results.
    /// </remarks>
    public static IReadOnlyList<TSource> Shuffle<TSource>(this IEnumerable<TSource> source)
        => source.Shuffle(RandomProvider.Shared);
#endif

    /// <summary>
    /// Returns the elements of the given sequence in random order, drawing the random numbers from <paramref name="random"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the enumerable.</typeparam>
    /// <remarks>
    /// This method is eager: the source is enumerated once when it is called, and the result is an independent list
    /// shuffled in O(n) with a Fisher-Yates shuffle, so every permutation is equally likely. The state of
    /// <paramref name="random"/> is advanced; two calls with the same instance return different orders, two calls
    /// with generators seeded alike return the same order.
    /// </remarks>
    public static IReadOnlyList<TSource> Shuffle<TSource>(this IEnumerable<TSource> source, Random random)
    {
        var materialized = source.ToArray();
        random.Shuffle(materialized);
        return materialized;
    }
}
