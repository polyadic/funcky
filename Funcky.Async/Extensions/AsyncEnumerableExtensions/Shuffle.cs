using static Funcky.Internal.Mixer;

namespace Funcky.Async.Extensions
{
    public static partial class AsyncEnumerableExtensions
    {
        /// <summary>
        /// Returns the given sequence in random Order in O(n).
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the enumerable.</typeparam>
        /// <remarks>This method is implemented by using deferred execution. The immediate return value is an object that stores all the information that is required to perform the action. The query represented by this method is not executed until the object is enumerated either by calling its GetEnumerator method directly or by using foreach.</remarks>
        [Pure]
        public static async ValueTask<IEnumerable<TSource>> Shuffle<TSource>(this IAsyncEnumerable<TSource> source)
            where TSource : notnull
            => ToRandomEnumerable(await source.ToListAsync().ConfigureAwait(false), new Random());
    }
}
