#if ASYNC_SUPPORTED
namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <param name="source">The source sequence.</param>
    /// <typeparam name="TSource">The type of the elements in the source sequence.</typeparam>
    extension<TSource>(IAsyncEnumerable<TSource> source)
    {
        /// <summary>
        /// Returns every nth (interval) element from the source sequence.
        /// </summary>
        /// <param name="interval">The interval between elements in the source sequences.</param>
        /// <returns>Returns a sequence where only every n'th element (interval) of source sequence is used. </returns>
        [Pure]
        public IAsyncEnumerable<TSource> TakeEvery(int interval)
        {
            ValidateInterval(interval);

            return source.Where((_, index) => index % interval == 0);
        }
    }

    private static void ValidateInterval(int interval)
    {
        if (interval <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(interval), interval, "Interval must be bigger than 0");
        }
    }
}
#endif
