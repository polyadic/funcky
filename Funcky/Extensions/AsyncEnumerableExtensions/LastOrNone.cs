#if INTEGRATED_ASYNC
namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <summary>
    /// Returns the last element of a sequence as an <see cref="Option{T}" />, or a <see cref="Option{T}.None" /> value if the sequence contains no elements.
    /// </summary>
    [Pure]
    public static async ValueTask<Option<TSource>> LastOrNoneAsync<TSource>(this IAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default)
        where TSource : notnull
        => await source.Select(Option.Some).LastOrDefaultAsync(cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Returns the last element of the sequence as an <see cref="Option{T}" /> that satisfies a condition or a <see cref="Option{T}.None" /> value if no such element is found.
    /// </summary>
    [Pure]
    public static async ValueTask<Option<TSource>> LastOrNoneAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, CancellationToken cancellationToken = default)
        where TSource : notnull
        => await source.Where(predicate).Select(Option.Some).LastOrDefaultAsync(cancellationToken).ConfigureAwait(false);

    /// <inheritdoc cref="LastOrNoneAsync{TSource}(IAsyncEnumerable{TSource},CancellationToken)"/>
    [Pure]
    public static async ValueTask<Option<TSource>> LastOrNoneAwaitAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, ValueTask<bool>> predicate, CancellationToken cancellationToken = default)
        where TSource : notnull
        => await source.Where((item, _) => predicate(item)).Select(Option.Some).LastOrDefaultAsync(cancellationToken).ConfigureAwait(false);

    /// <inheritdoc cref="LastOrNoneAsync{TSource}(IAsyncEnumerable{TSource},CancellationToken)"/>
    [Pure]
    public static async ValueTask<Option<TSource>> LastOrNoneAwaitWithCancellationAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<bool>> predicate, CancellationToken cancellationToken = default)
        where TSource : notnull
        => await source.Where(predicate).Select(Option.Some).LastOrDefaultAsync(cancellationToken).ConfigureAwait(false);
}
#endif
