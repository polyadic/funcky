#if ASYNC_SUPPORTED
namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    extension<TSource>(IAsyncEnumerable<TSource> source)
        where TSource : notnull
    {
        /// <summary>
        /// Returns the only element of a sequence as an <see cref="Option{T}" />, or a <see cref="Option{T}.None" /> value if the sequence is empty.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if there is more than one element in the sequence.</exception>
        [Pure]
        public async ValueTask<Option<TSource>> SingleOrNoneAsync(CancellationToken cancellationToken = default)
            => await source.Select(Option.Some).SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Returns the only element of a sequence that satisfies a specified condition as an <see cref="Option{T}" /> or a <see cref="Option{T}.None" /> value if no such element exists.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if more than one element satisfies the condition.</exception>
        [Pure]
        public async ValueTask<Option<TSource>> SingleOrNoneAsync(Func<TSource, bool> predicate, CancellationToken cancellationToken = default)
            => await source.Where(predicate).Select(Option.Some).SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Returns the only element of a sequence that satisfies a specified condition as an <see cref="Option{T}" /> or a <see cref="Option{T}.None" /> value if no such element exists.
        /// </summary>
        /// <param name="predicate">An asynchronous predicate; it receives the cancellation token.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <exception cref="InvalidOperationException">Thrown if more than one element satisfies the condition.</exception>
        [Pure]
        public async ValueTask<Option<TSource>> SingleOrNoneAsync(Func<TSource, CancellationToken, ValueTask<bool>> predicate, CancellationToken cancellationToken = default)
            => await source.Where(predicate).Select(Option.Some).SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false);
    }
}
#endif
