#if ASYNC_SUPPORTED
using System.Runtime.CompilerServices;
using Funcky.Internal.Validators;

namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <param name="source">The source sequence.</param>
    /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    extension<TSource>(IAsyncEnumerable<TSource> source)
    {
        /// <summary>
        /// Chunks the source sequence into equally sized chunks. The last chunk can be smaller.
        /// </summary>
        /// <typeparam name="TResult">Type of the elements returned.</typeparam>
        /// <param name="size">The desired size of the chunks.</param>
        /// <param name="resultSelector">The result selector will be applied on each chunked sequence and can produce a desired result.</param>
        /// <returns>A sequence of results based on equally sized chunks.</returns>
        [Pure]
        public IAsyncEnumerable<TResult> Chunk<TResult>(int size, Func<IReadOnlyList<TSource>, TResult> resultSelector)
            => ChunkEnumerable(ArgumentValidator.NotNull(source), ChunkSizeValidator.Validate(size))
                .Select(ArgumentValidator.NotNull(resultSelector));

        /// <summary>
        /// Chunks the source sequence into equally sized chunks. The last chunk can be smaller.
        /// </summary>
        /// <typeparam name="TResult">Type of the elements returned.</typeparam>
        /// <param name="size">The desired size of the chunks.</param>
        /// <param name="resultSelector">The asynchronous result selector will be applied on each chunked sequence and can produce a desired result; it receives the cancellation token of the enumeration.</param>
        /// <returns>A sequence of results based on equally sized chunks.</returns>
        [Pure]
        [OverloadResolutionPriority(1)]
        public IAsyncEnumerable<TResult> Chunk<TResult>(int size, Func<IReadOnlyList<TSource>, CancellationToken, ValueTask<TResult>> resultSelector)
            => ChunkEnumerable(ArgumentValidator.NotNull(source), ChunkSizeValidator.Validate(size))
                .Select(ArgumentValidator.NotNull(resultSelector));
    }

    private static async IAsyncEnumerable<IReadOnlyList<TSource>> ChunkEnumerable<TSource>(IAsyncEnumerable<TSource> source, int size, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var asyncEnumerator = source.GetAsyncEnumerator(cancellationToken);
        await using var sourceEnumerator = asyncEnumerator.ConfigureAwait(false);

        while (await asyncEnumerator.MoveNextAsync().ConfigureAwait(false))
        {
            yield return await TakeSkipAsync(asyncEnumerator, size).ToListAsync(cancellationToken).ConfigureAwait(false);
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
#endif
