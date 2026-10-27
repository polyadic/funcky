#if ASYNC_SUPPORTED
namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    extension<TSource>(IAsyncEnumerable<TSource> source)
    {
        /// <summary>Determines whether a sequence contains no elements.</summary>
        [Pure]
        public async ValueTask<bool> NoneAsync(CancellationToken cancellationToken = default)
            => !await source.AnyAsync(cancellationToken).ConfigureAwait(false);

        /// <summary>Determines whether no element exists or satisfies a condition.</summary>
        [Pure]
        public async ValueTask<bool> NoneAsync(Func<TSource, bool> predicate, CancellationToken cancellationToken = default)
            => !await source.AnyAsync(predicate, cancellationToken).ConfigureAwait(false);

        /// <summary>Determines whether no element exists or satisfies a condition.</summary>
        /// <param name="predicate">An asynchronous predicate; it receives the cancellation token.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        [Pure]
        public async ValueTask<bool> NoneAsync(Func<TSource, CancellationToken, ValueTask<bool>> predicate, CancellationToken cancellationToken = default)
            => !await source.AnyAsync(predicate, cancellationToken).ConfigureAwait(false);

        /// <inheritdoc cref="NoneAsync{TSource}(IAsyncEnumerable{TSource}, Func{TSource,bool}, CancellationToken)"/>
        [Pure]
        [Obsolete("Use NoneAsync with a (item, cancellationToken) delegate instead; discard the token with (item, _) => …. This method will be removed in Funcky 4.0.0.")]
        public ValueTask<bool> NoneAwaitAsync(Func<TSource, ValueTask<bool>> predicate, CancellationToken cancellationToken = default)
            => source.NoneAsync((item, _) => predicate(item), cancellationToken);

        /// <inheritdoc cref="NoneAsync{TSource}(IAsyncEnumerable{TSource}, Func{TSource,bool}, CancellationToken)"/>
        [Pure]
        [Obsolete("Use NoneAsync instead, which takes the same delegate. This method will be removed in Funcky 4.0.0.")]
        public ValueTask<bool> NoneAwaitWithCancellationAsync(Func<TSource, CancellationToken, ValueTask<bool>> predicate, CancellationToken cancellationToken = default)
            => source.NoneAsync(predicate, cancellationToken);
    }
}
#endif
