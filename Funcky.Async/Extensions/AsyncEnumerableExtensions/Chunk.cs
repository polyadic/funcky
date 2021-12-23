namespace Funcky.Async.Extensions
{
    public static partial class AsyncEnumerableExtensions
    {
        /// <summary>
        /// Chunks the source sequence into equally sized chunks. The last chunk can be smaller.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="size">The desired size of the chunks.</param>
        /// <returns>A sequence of equally sized sequences containing elements of the source collection in the same order.</returns>
        [Pure]
        public static IAsyncEnumerable<IReadOnlyList<TSource>> Chunk<TSource>(this IAsyncEnumerable<TSource> source, int size)
            => ChunkEnumerable(source, ValidateChunkSize(size));

        /// <summary>
        /// Chunks the source sequence into equally sized chunks. The last chunk can be smaller.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
        /// <typeparam name="TResult">Type of the elements returned.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="size">The desired size of the chunks.</param>
        /// <param name="resultSelector">The result selector will be applied on each chunked sequence and can produce a desired result.</param>
        /// <returns>A sequence of results based on equally sized chunks.</returns>
        [Pure]
        public static IAsyncEnumerable<TResult> Chunk<TSource, TResult>(this IAsyncEnumerable<TSource> source, int size, Func<IReadOnlyList<TSource>, TResult> resultSelector)
            => ChunkEnumerable(source, ValidateChunkSize(size))
                .Select(resultSelector);

        /// <summary>
        /// Chunks the source sequence into equally sized chunks. The last chunk can be smaller.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
        /// <typeparam name="TResult">Type of the elements returned.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="size">The desired size of the chunks.</param>
        /// <param name="resultSelector">The result selector will be applied on each chunked sequence and can produce a desired result.</param>
        /// <returns>A sequence of results based on equally sized chunks.</returns>
        [Pure]
        public static IAsyncEnumerable<TResult> ChunkAwait<TSource, TResult>(this IAsyncEnumerable<TSource> source, int size, Func<IReadOnlyList<TSource>, ValueTask<TResult>> resultSelector)
            => ChunkEnumerable(source, ValidateChunkSize(size))
                .SelectAwait(resultSelector);

        /// <summary>
        /// Chunks the source sequence into equally sized chunks. The last chunk can be smaller.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
        /// <typeparam name="TResult">Type of the elements returned.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="size">The desired size of the chunks.</param>
        /// <param name="resultSelector">The result selector will be applied on each chunked sequence and can produce a desired result.</param>
        /// <returns>A sequence of results based on equally sized chunks.</returns>
        [Pure]
        public static IAsyncEnumerable<TResult> ChunkAwaitWithCancellation<TSource, TResult>(this IAsyncEnumerable<TSource> source, int size, Func<IReadOnlyList<TSource>, CancellationToken, ValueTask<TResult>> resultSelector)
            => ChunkEnumerable(source, ValidateChunkSize(size))
                .SelectAwaitWithCancellation(resultSelector);

        private static int ValidateChunkSize(int size)
            => size > 0
                ? size
                : throw new ArgumentOutOfRangeException(nameof(size), size, "Size must be bigger than 0");

        private static async IAsyncEnumerable<IReadOnlyList<TSource>> ChunkEnumerable<TSource>(IAsyncEnumerable<TSource> source, int size)
        {
            var asyncEnumerator = source.GetAsyncEnumerator();
            await using var sourceEnumerator = asyncEnumerator.ConfigureAwait(false);

            while (await asyncEnumerator.MoveNextAsync().ConfigureAwait(false))
            {
                yield return await TakeSkipAsync(asyncEnumerator, size).ToListAsync().ConfigureAwait(false);
            }
        }

        private static async IAsyncEnumerable<TSource> TakeSkipAsync<TSource>(IAsyncEnumerator<TSource> source, int size)
        {
            do
            {
                yield return source.Current;
            }
            while (--size > 0 && await source.MoveNextAsync().ConfigureAwait(false));
        }
    }
}
