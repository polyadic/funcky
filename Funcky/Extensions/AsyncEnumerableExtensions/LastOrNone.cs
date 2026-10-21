#if ASYNC_SUPPORTED
namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    extension<TSource>(IAsyncEnumerable<TSource> source)
        where TSource : notnull
    {
        /// <summary>
        /// Returns the last element of a sequence as an <see cref="Option{T}" />, or a <see cref="Option{T}.None" /> value if the sequence contains no elements.
        /// </summary>
        [Pure]
        public async ValueTask<Option<TSource>> LastOrNoneAsync(CancellationToken cancellationToken = default)
            => await source.Select(Option.Some).LastOrDefaultAsync(cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Returns the last element of the sequence as an <see cref="Option{T}" /> that satisfies a condition or a <see cref="Option{T}.None" /> value if no such element is found.
        /// </summary>
        [Pure]
        public async ValueTask<Option<TSource>> LastOrNoneAsync(Func<TSource, bool> predicate, CancellationToken cancellationToken = default)
            => await source.Where(predicate).Select(Option.Some).LastOrDefaultAsync(cancellationToken).ConfigureAwait(false);

        /// <inheritdoc cref="LastOrNoneAsync{TSource}(IAsyncEnumerable{TSource},CancellationToken)"/>
        [Pure]
        public async ValueTask<Option<TSource>> LastOrNoneAwaitAsync(Func<TSource, ValueTask<bool>> predicate, CancellationToken cancellationToken = default)
            => await source.Where((item, _) => predicate(item)).Select(Option.Some).LastOrDefaultAsync(cancellationToken).ConfigureAwait(false);

        /// <inheritdoc cref="LastOrNoneAsync{TSource}(IAsyncEnumerable{TSource},CancellationToken)"/>
        [Pure]
        public async ValueTask<Option<TSource>> LastOrNoneAwaitWithCancellationAsync(Func<TSource, CancellationToken, ValueTask<bool>> predicate, CancellationToken cancellationToken = default)
            => await source.Where(predicate).Select(Option.Some).LastOrDefaultAsync(cancellationToken).ConfigureAwait(false);
    }
}
#endif
