#if ASYNC_SUPPORTED
namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    extension<TSource>(IAsyncEnumerable<TSource> source)
        where TSource : notnull
    {
        /// <summary>
        /// Returns the first element of a sequence as an <see cref="Option{T}" />, or a <see cref="Option{T}.None" /> value if the sequence contains no elements.
        /// </summary>
        [Pure]
        public async ValueTask<Option<TSource>> FirstOrNoneAsync(CancellationToken cancellationToken = default)
            => await source.Select(Option.Some).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Returns the first element of the sequence as an <see cref="Option{T}" /> that satisfies a condition or a <see cref="Option{T}.None" /> value if no such element is found.
        /// </summary>
        [Pure]
        public async ValueTask<Option<TSource>> FirstOrNoneAsync(Func<TSource, bool> predicate, CancellationToken cancellationToken = default)
            => await source.Where(predicate).Select(Option.Some).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Returns the first element of the sequence as an <see cref="Option{T}" /> that satisfies a condition or a <see cref="Option{T}.None" /> value if no such element is found.
        /// </summary>
        /// <param name="predicate">An asynchronous predicate; it receives the cancellation token.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        [Pure]
        public async ValueTask<Option<TSource>> FirstOrNoneAsync(Func<TSource, CancellationToken, ValueTask<bool>> predicate, CancellationToken cancellationToken = default)
            => await source.Where(predicate).Select(Option.Some).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);

        /// <inheritdoc cref="FirstOrNoneAsync{TSource}(IAsyncEnumerable{TSource},CancellationToken)"/>
        [Pure]
        [Obsolete("Use FirstOrNoneAsync with a (item, cancellationToken) delegate instead; discard the token with (item, _) => …. This method will be removed in Funcky 4.0.0.")]
        public ValueTask<Option<TSource>> FirstOrNoneAwaitAsync(Func<TSource, ValueTask<bool>> predicate, CancellationToken cancellationToken = default)
            => source.FirstOrNoneAsync((item, _) => predicate(item), cancellationToken);

        /// <inheritdoc cref="FirstOrNoneAsync{TSource}(IAsyncEnumerable{TSource},CancellationToken)"/>
        [Pure]
        [Obsolete("Use FirstOrNoneAsync instead, which takes the same delegate. This method will be removed in Funcky 4.0.0.")]
        public ValueTask<Option<TSource>> FirstOrNoneAwaitWithCancellationAsync(Func<TSource, CancellationToken, ValueTask<bool>> predicate, CancellationToken cancellationToken = default)
            => source.FirstOrNoneAsync(predicate, cancellationToken);
    }
}
#endif
