#if INTEGRATED_ASYNC
namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <summary>Determines whether a sequence contains no elements.</summary>
    [Pure]
    public static async ValueTask<bool> NoneAsync<TSource>(this IAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default)
        => !await source.AnyAsync(cancellationToken).ConfigureAwait(false);

    /// <summary>Determines whether no element exists or satisfies a condition.</summary>
    [Pure]
    public static async ValueTask<bool> NoneAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, CancellationToken cancellationToken = default)
        => !await source.AnyAsync(predicate, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc cref="NoneAsync{TSource}(IAsyncEnumerable{TSource}, Func{TSource,bool}, CancellationToken)"/>
    [Pure]
    public static async ValueTask<bool> NoneAwaitAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, ValueTask<bool>> predicate, CancellationToken cancellationToken = default)
        => !await source.AnyAsync((item, _) => predicate(item), cancellationToken).ConfigureAwait(false);

    /// <inheritdoc cref="NoneAsync{TSource}(IAsyncEnumerable{TSource}, Func{TSource,bool}, CancellationToken)"/>
    [Pure]
    public static async ValueTask<bool> NoneAwaitWithCancellationAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<bool>> predicate, CancellationToken cancellationToken = default)
        => !await source.AnyAsync(predicate, cancellationToken).ConfigureAwait(false);
}
#endif
