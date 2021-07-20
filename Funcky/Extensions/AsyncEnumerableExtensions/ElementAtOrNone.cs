namespace Funcky.Extensions
{
    public static partial class AsyncEnumerableExtensions
    {
        /// <summary>
        /// Returns the element at a specified index in a sequence or an <see cref="Option{T}.None" /> value if the index is out of range.
        /// </summary>
        /// <typeparam name="TSource">The type of element contained by the sequence.</typeparam>
        /// <param name="source">The sequence to find an element in.</param>
        /// <param name="index">The index for the element to retrieve.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The item at the specified index, or <see cref="Option{T}.None" /> if the index is not found.</returns>
        [Pure]
        public static async ValueTask<Option<TSource>> ElementAtOrNoneAsync<TSource>(this IAsyncEnumerable<TSource> source, int index, CancellationToken cancellationToken = default)
            where TSource : notnull
        {
            var currentIndex = 0;
            await foreach (var item in source.WithCancellation(cancellationToken))
            {
                if (currentIndex == index)
                {
                    return item;
                }

                currentIndex++;
            }

            return Option<TSource>.None();
        }
    }
}
