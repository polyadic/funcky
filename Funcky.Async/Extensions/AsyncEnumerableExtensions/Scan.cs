using System.Runtime.CompilerServices;

namespace Funcky.Async.Extensions
{
    public static partial class AsyncEnumerableExtensions
    {
        /// <summary>
        /// Scan generates a sequence known as the the inclusive prefix sum.
        /// </summary>
        /// <typeparam name="TSource">The type of the source elements.</typeparam>
        /// <typeparam name="TAccumulate">The seed and target type.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="seed">The seed or neutral element (identity).</param>
        /// <param name="accumulator">A binary operator to aggregate a value.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A sequence of aggregated values.</returns>
        public static async IAsyncEnumerable<TAccumulate> InclusiveScan<TSource, TAccumulate>(this IAsyncEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> accumulator, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var enumerator = source.GetAsyncEnumerator(cancellationToken);
            await using var enumeratorGuard = enumerator.ConfigureAwait(false);

            while (await enumerator.MoveNextAsync().ConfigureAwait(false))
            {
                seed = accumulator(seed, enumerator.Current);
                yield return seed;
            }
        }

        /// <summary>
        /// Scan generates a sequence known as the the inclusive prefix sum.
        /// </summary>
        /// <typeparam name="TSource">The type of the source elements.</typeparam>
        /// <typeparam name="TAccumulate">The seed and target type.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="seed">The seed or neutral element (identity).</param>
        /// <param name="accumulator">An awaitable binary operator to aggregate a value.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A sequence of aggregated values.</returns>
        public static async IAsyncEnumerable<TAccumulate> InclusiveScanAwait<TSource, TAccumulate>(this IAsyncEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, ValueTask<TAccumulate>> accumulator, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var enumerator = source.GetAsyncEnumerator(cancellationToken);
            await using var enumeratorGuard = enumerator.ConfigureAwait(false);

            while (await enumerator.MoveNextAsync().ConfigureAwait(false))
            {
                seed = await accumulator(seed, enumerator.Current).ConfigureAwait(false);
                yield return seed;
            }
        }

        /// <summary>
        /// Scan generates a sequence known as the the inclusive prefix sum.
        /// </summary>
        /// <typeparam name="TSource">The type of the source elements.</typeparam>
        /// <typeparam name="TAccumulate">The seed and target type.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="seed">The seed or neutral element (identity).</param>
        /// <param name="accumulator">A binary operator to aggregate a value.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A sequence of aggregated values.</returns>
        public static async IAsyncEnumerable<TAccumulate> InclusiveScanAwaitWithCancellation<TSource, TAccumulate>(this IAsyncEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, CancellationToken, ValueTask<TAccumulate>> accumulator, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var enumerator = source.GetAsyncEnumerator(cancellationToken);
            await using var enumeratorGuard = enumerator.ConfigureAwait(false);

            while (await enumerator.MoveNextAsync().ConfigureAwait(false))
            {
                seed = await accumulator(seed, enumerator.Current, cancellationToken).ConfigureAwait(false);
                yield return seed;
            }
        }

        /// <summary>
        /// Scan generates a sequence known as the the exclusive prefix sum.
        /// </summary>
        /// <typeparam name="TSource">The type of the source elements.</typeparam>
        /// <typeparam name="TAccumulate">The seed and target type.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="seed">The seed or neutral element (identity).</param>
        /// <param name="accumulator">a binary operator to aggregate a value.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A sequence of aggregated values.</returns>
        public static async IAsyncEnumerable<TAccumulate> ExclusiveScan<TSource, TAccumulate>(this IAsyncEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> accumulator, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var enumerator = source.GetAsyncEnumerator(cancellationToken);
            await using var enumeratorGuard = enumerator.ConfigureAwait(false);

            while (await enumerator.MoveNextAsync().ConfigureAwait(false))
            {
                yield return seed;
                seed = accumulator(seed, enumerator.Current);
            }
        }

        /// <summary>
        /// Scan generates a sequence known as the the exclusive prefix sum.
        /// </summary>
        /// <typeparam name="TSource">The type of the source elements.</typeparam>
        /// <typeparam name="TAccumulate">The seed and target type.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="seed">The seed or neutral element (identity).</param>
        /// <param name="accumulator">An awaitable binary operator to aggregate a value.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A sequence of aggregated values.</returns>
        public static async IAsyncEnumerable<TAccumulate> ExclusiveScanAwait<TSource, TAccumulate>(this IAsyncEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, ValueTask<TAccumulate>> accumulator, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var enumerator = source.GetAsyncEnumerator(cancellationToken);
            await using var enumeratorGuard = enumerator.ConfigureAwait(false);

            while (await enumerator.MoveNextAsync().ConfigureAwait(false))
            {
                yield return seed;
                seed = await accumulator(seed, enumerator.Current).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Scan generates a sequence known as the the exclusive prefix sum.
        /// </summary>
        /// <typeparam name="TSource">The type of the source elements.</typeparam>
        /// <typeparam name="TAccumulate">The seed and target type.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="seed">The seed or neutral element (identity).</param>
        /// <param name="accumulator">An awaitable binary operator to aggregate a value.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A sequence of aggregated values.</returns>
        public static async IAsyncEnumerable<TAccumulate> ExclusiveScanAwaitWithCancellation<TSource, TAccumulate>(this IAsyncEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, CancellationToken, ValueTask<TAccumulate>> accumulator, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var enumerator = source.GetAsyncEnumerator(cancellationToken);
            await using var enumeratorGuard = enumerator.ConfigureAwait(false);

            while (await enumerator.MoveNextAsync().ConfigureAwait(false))
            {
                yield return seed;
                seed = await accumulator(seed, enumerator.Current, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
