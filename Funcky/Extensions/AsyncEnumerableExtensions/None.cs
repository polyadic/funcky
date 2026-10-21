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

        /// <inheritdoc cref="NoneAsync{TSource}(IAsyncEnumerable{TSource}, Func{TSource,bool}, CancellationToken)"/>
        [Pure]
        public async ValueTask<bool> NoneAwaitAsync(Func<TSource, ValueTask<bool>> predicate, CancellationToken cancellationToken = default)
            => !await source.AnyAsync((item, _) => predicate(item), cancellationToken).ConfigureAwait(false);

        /// <inheritdoc cref="NoneAsync{TSource}(IAsyncEnumerable{TSource}, Func{TSource,bool}, CancellationToken)"/>
        [Pure]
        public async ValueTask<bool> NoneAwaitWithCancellationAsync(Func<TSource, CancellationToken, ValueTask<bool>> predicate, CancellationToken cancellationToken = default)
            => !await source.AnyAsync(predicate, cancellationToken).ConfigureAwait(false);
    }
}
#endif
