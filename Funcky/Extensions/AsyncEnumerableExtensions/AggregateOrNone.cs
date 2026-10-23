#if ASYNC_SUPPORTED
using Funcky.Internal.Validators;

namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <param name="source">A sequence to aggregate over.</param>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    extension<TSource>(IAsyncEnumerable<TSource> source)
        where TSource : notnull
    {
        /// <summary>
        /// Applies an accumulator function over a sequence, using the first element as the seed. If the sequence is empty it returns None.
        /// </summary>
        /// <remarks>
        /// This is the seedless <c>AggregateAsync</c>, which throws for an empty sequence.
        /// The accumulator function is never invoked for a sequence with a single element.
        /// </remarks>
        /// <param name="func">An accumulator function to be invoked on each element after the first.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>The final accumulator value or None.</returns>
        [Pure]
        public ValueTask<Option<TSource>> AggregateOrNoneAsync(Func<TSource, TSource, TSource> func, CancellationToken cancellationToken = default)
        {
            ArgumentValidator.NotNull(source);
            ArgumentValidator.NotNull(func);

            return AggregateOrNoneInternal(source, (accumulator, item, _) => new ValueTask<TSource>(func(accumulator, item)), cancellationToken);
        }

        /// <summary>
        /// Applies an asynchronous accumulator function over a sequence, using the first element as the seed. If the sequence is empty it returns None.
        /// </summary>
        /// <remarks>
        /// The accumulator function receives the cancellation token of the operation. It is never invoked for a sequence with a single element.
        /// </remarks>
        /// <param name="func">An asynchronous accumulator function to be invoked on each element after the first.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>The final accumulator value or None.</returns>
        [Pure]
        public ValueTask<Option<TSource>> AggregateOrNoneAsync(Func<TSource, TSource, CancellationToken, ValueTask<TSource>> func, CancellationToken cancellationToken = default)
        {
            ArgumentValidator.NotNull(source);
            ArgumentValidator.NotNull(func);

            return AggregateOrNoneInternal(source, func, cancellationToken);
        }
    }

    private static async ValueTask<Option<TSource>> AggregateOrNoneInternal<TSource>(IAsyncEnumerable<TSource> source, Func<TSource, TSource, CancellationToken, ValueTask<TSource>> func, CancellationToken cancellationToken)
        where TSource : notnull
    {
#pragma warning disable CA2007
        await using var enumerator = source.ConfigureAwait(false).WithCancellation(cancellationToken).GetAsyncEnumerator();
#pragma warning restore CA2007

        if (!await enumerator.MoveNextAsync())
        {
            return Option<TSource>.None;
        }

        var accumulator = enumerator.Current;

        while (await enumerator.MoveNextAsync())
        {
            accumulator = await func(accumulator, enumerator.Current, cancellationToken).ConfigureAwait(false);
        }

        return accumulator;
    }
}
#endif
