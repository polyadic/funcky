#if ASYNC_SUPPORTED
using System.Runtime.CompilerServices;
using Funcky.Internal.Validators;

namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <param name="source">The source sequence.</param>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    extension<TSource>(IAsyncEnumerable<TSource> source)
    {
        /// <summary>
        /// Scan generates a sequence known as the inclusive prefix sum.
        /// </summary>
        /// <typeparam name="TAccumulate">The seed and target type.</typeparam>
        /// <param name="seed">The seed or neutral element (identity).</param>
        /// <param name="accumulator">A binary operator to aggregate a value.</param>
        /// <returns>A sequence of aggregated values.</returns>
        public IAsyncEnumerable<TAccumulate> InclusiveScan<TAccumulate>(TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> accumulator)
            => InclusiveScanEnumerable(ArgumentValidator.NotNull(source), seed, ArgumentValidator.NotNull(accumulator));

        /// <summary>
        /// Scan generates a sequence known as the inclusive prefix sum.
        /// </summary>
        /// <typeparam name="TAccumulate">The seed and target type.</typeparam>
        /// <param name="seed">The seed or neutral element (identity).</param>
        /// <param name="accumulator">An awaitable binary operator to aggregate a value.</param>
        /// <returns>A sequence of aggregated values.</returns>
        public IAsyncEnumerable<TAccumulate> InclusiveScanAwait<TAccumulate>(TAccumulate seed, Func<TAccumulate, TSource, ValueTask<TAccumulate>> accumulator)
            => InclusiveScanAwaitEnumerable(ArgumentValidator.NotNull(source), seed, ArgumentValidator.NotNull(accumulator));

        /// <summary>
        /// Scan generates a sequence known as the inclusive prefix sum.
        /// </summary>
        /// <typeparam name="TAccumulate">The seed and target type.</typeparam>
        /// <param name="seed">The seed or neutral element (identity).</param>
        /// <param name="accumulator">A binary operator to aggregate a value.</param>
        /// <returns>A sequence of aggregated values.</returns>
        public IAsyncEnumerable<TAccumulate> InclusiveScanAwaitWithCancellation<TAccumulate>(TAccumulate seed, Func<TAccumulate, TSource, CancellationToken, ValueTask<TAccumulate>> accumulator)
            => InclusiveScanAwaitWithCancellationEnumerable(ArgumentValidator.NotNull(source), seed, ArgumentValidator.NotNull(accumulator));

        /// <summary>
        /// Scan generates a sequence known as the exclusive prefix sum.
        /// </summary>
        /// <typeparam name="TAccumulate">The seed and target type.</typeparam>
        /// <param name="seed">The seed or neutral element (identity).</param>
        /// <param name="accumulator">a binary operator to aggregate a value.</param>
        /// <returns>A sequence of aggregated values.</returns>
        public IAsyncEnumerable<TAccumulate> ExclusiveScan<TAccumulate>(TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> accumulator)
            => ExclusiveScanEnumerable(ArgumentValidator.NotNull(source), seed, ArgumentValidator.NotNull(accumulator));

        /// <summary>
        /// Scan generates a sequence known as the exclusive prefix sum.
        /// </summary>
        /// <typeparam name="TAccumulate">The seed and target type.</typeparam>
        /// <param name="seed">The seed or neutral element (identity).</param>
        /// <param name="accumulator">An awaitable binary operator to aggregate a value.</param>
        /// <returns>A sequence of aggregated values.</returns>
        public IAsyncEnumerable<TAccumulate> ExclusiveScanAwait<TAccumulate>(TAccumulate seed, Func<TAccumulate, TSource, ValueTask<TAccumulate>> accumulator)
            => ExclusiveScanAwaitEnumerable(ArgumentValidator.NotNull(source), seed, ArgumentValidator.NotNull(accumulator));

        /// <summary>
        /// Scan generates a sequence known as the exclusive prefix sum.
        /// </summary>
        /// <typeparam name="TAccumulate">The seed and target type.</typeparam>
        /// <param name="seed">The seed or neutral element (identity).</param>
        /// <param name="accumulator">An awaitable binary operator to aggregate a value.</param>
        /// <returns>A sequence of aggregated values.</returns>
        public IAsyncEnumerable<TAccumulate> ExclusiveScanAwaitWithCancellation<TAccumulate>(TAccumulate seed, Func<TAccumulate, TSource, CancellationToken, ValueTask<TAccumulate>> accumulator)
            => ExclusiveScanAwaitWithCancellationEnumerable(ArgumentValidator.NotNull(source), seed, ArgumentValidator.NotNull(accumulator));

        private async IAsyncEnumerable<TAccumulate> InclusiveScanAwaitWithCancellationEnumerable<TAccumulate>(TAccumulate seed, Func<TAccumulate, TSource, CancellationToken, ValueTask<TAccumulate>> accumulator, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var enumerator = source.GetAsyncEnumerator(cancellationToken);
            await using var enumeratorGuard = enumerator.ConfigureAwait(false);

            while (await enumerator.MoveNextAsync().ConfigureAwait(false))
            {
                seed = await accumulator(seed, enumerator.Current, cancellationToken).ConfigureAwait(false);
                yield return seed;
            }
        }

        private async IAsyncEnumerable<TAccumulate> ExclusiveScanEnumerable<TAccumulate>(TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> accumulator, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var enumerator = source.GetAsyncEnumerator(cancellationToken);
            await using var enumeratorGuard = enumerator.ConfigureAwait(false);

            while (await enumerator.MoveNextAsync().ConfigureAwait(false))
            {
                yield return seed;
                seed = accumulator(seed, enumerator.Current);
            }
        }

        private async IAsyncEnumerable<TAccumulate> ExclusiveScanAwaitEnumerable<TAccumulate>(TAccumulate seed, Func<TAccumulate, TSource, ValueTask<TAccumulate>> accumulator, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var enumerator = source.GetAsyncEnumerator(cancellationToken);
            await using var enumeratorGuard = enumerator.ConfigureAwait(false);

            while (await enumerator.MoveNextAsync().ConfigureAwait(false))
            {
                yield return seed;
                seed = await accumulator(seed, enumerator.Current).ConfigureAwait(false);
            }
        }

        private async IAsyncEnumerable<TAccumulate> ExclusiveScanAwaitWithCancellationEnumerable<TAccumulate>(TAccumulate seed, Func<TAccumulate, TSource, CancellationToken, ValueTask<TAccumulate>> accumulator, [EnumeratorCancellation] CancellationToken cancellationToken = default)
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

    private static async IAsyncEnumerable<TAccumulate> InclusiveScanEnumerable<TSource, TAccumulate>(IAsyncEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> accumulator, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var enumerator = source.GetAsyncEnumerator(cancellationToken);
        await using var enumeratorGuard = enumerator.ConfigureAwait(false);

        while (await enumerator.MoveNextAsync().ConfigureAwait(false))
        {
            seed = accumulator(seed, enumerator.Current);
            yield return seed;
        }
    }

    private static async IAsyncEnumerable<TAccumulate> InclusiveScanAwaitEnumerable<TSource, TAccumulate>(IAsyncEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, ValueTask<TAccumulate>> accumulator, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var enumerator = source.GetAsyncEnumerator(cancellationToken);
        await using var enumeratorGuard = enumerator.ConfigureAwait(false);

        while (await enumerator.MoveNextAsync().ConfigureAwait(false))
        {
            seed = await accumulator(seed, enumerator.Current).ConfigureAwait(false);
            yield return seed;
        }
    }
}
#endif
