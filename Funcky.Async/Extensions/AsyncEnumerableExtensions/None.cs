namespace Funcky.Async.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <summary>Determines whether a sequence contains no elements.</summary>
    [Pure]
    public static async ValueTask<bool> NoneAsync<TSource>(this IAsyncEnumerable<TSource> source)
        => !await source.AnyAsync().ConfigureAwait(false);

    /// <summary>Determines whether no element exists or satisfies a condition.</summary>
    [Pure]
    public static async ValueTask<bool> NoneAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate)
        => !await source.AnyAsync(predicate).ConfigureAwait(false);

    /// <inheritdoc cref="NoneAsync{TSource}(System.Collections.Generic.IAsyncEnumerable{TSource}, Func{TSource,bool})"/>
    [Pure]
    public static async ValueTask<bool> NoneAwaitAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, ValueTask<bool>> predicate)
        => !await source.AnyAwaitAsync(predicate).ConfigureAwait(false);

    /// <inheritdoc cref="NoneAsync{TSource}(System.Collections.Generic.IAsyncEnumerable{TSource}, Func{TSource,bool})"/>
    [Pure]
    public static async ValueTask<bool> NoneAwaitWithCancellationAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<bool>> predicate)
        => !await source.AnyAwaitWithCancellationAsync(predicate).ConfigureAwait(false);
}
