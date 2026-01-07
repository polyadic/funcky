namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <summary>
    /// Returns the only element of a sequence as an <see cref="Option{T}" />, or a <see cref="Option{T}.None" /> value if the sequence is empty.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if there is more than one element in the sequence.</exception>
    [Pure]
    public static async ValueTask<Option<TSource>> SingleOrNoneAsync<TSource>(this IAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default)
        where TSource : notnull
        => await source.Select(Option.Some).SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Returns the only element of a sequence that satisfies a specified condition as an <see cref="Option{T}" /> or a <see cref="Option{T}.None" /> value if no such element exists.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if more than one element satisfies the condition.</exception>
    [Pure]
    public static async ValueTask<Option<TSource>> SingleOrNoneAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, CancellationToken cancellationToken = default)
        where TSource : notnull
        => await source.Where(predicate).Select(Option.Some).SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false);

    /// <inheritdoc cref="SingleOrNoneAsync{TSource}(IAsyncEnumerable{TSource},CancellationToken)"/>
    [Pure]
    public static async ValueTask<Option<TSource>> SingleOrNoneAwaitAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, ValueTask<bool>> predicate, CancellationToken cancellationToken = default)
        where TSource : notnull
        => await source.WhereAwait(predicate).Select(Option.Some).SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false);

    /// <inheritdoc cref="SingleOrNoneAsync{TSource}(IAsyncEnumerable{TSource},CancellationToken)"/>
    [Pure]
    public static async ValueTask<Option<TSource>> SingleOrNoneAwaitWithCancellationAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<bool>> predicate, CancellationToken cancellationToken = default)
        where TSource : notnull
        => await source.WhereAwaitWithCancellation(predicate).Select(Option.Some).SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false);
}
