#if !SHUFFLE_EXTENSION
using Funcky.Internal;
#endif

namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
#if !SHUFFLE_EXTENSION
    /// <typeparam name="TSource">The type of the elements in the enumerable.</typeparam>
    extension<TSource>(IEnumerable<TSource> source)
    {
        /// <summary>
        /// Returns the elements of the given sequence in random order.
        /// </summary>
        /// <remarks>
        /// This method is eager: the source is enumerated once when it is called, and the result is an independent list
        /// shuffled in O(n) with a Fisher-Yates shuffle, so every permutation is equally likely. The random numbers come
        /// from a shared generator; pass a <see cref="Random"/> for reproducible results.
        /// </remarks>
        public IReadOnlyList<TSource> Shuffle()
            => source.Shuffle(RandomProvider.Shared);
    }
#endif
    /// <typeparam name="TSource">The type of the elements in the enumerable.</typeparam>
    extension<TSource>(IEnumerable<TSource> source)
    {
        /// <summary>
        /// Returns the elements of the given sequence in random order, drawing the random numbers from <paramref name="random"/>.
        /// </summary>
        /// <remarks>
        /// This method is eager: the source is enumerated once when it is called, and the result is an independent list
        /// shuffled in O(n) with a Fisher-Yates shuffle, so every permutation is equally likely. The state of
        /// <paramref name="random"/> is advanced; two calls with the same instance return different orders, two calls
        /// with generators seeded alike return the same order.
        /// </remarks>
        public IReadOnlyList<TSource> Shuffle(Random random)
        {
            var materialized = source.ToArray();
            random.Shuffle(materialized);
            return materialized;
        }
    }
}
