namespace Funcky.Extensions;

/// <summary>
/// .NET 10 operator shapes used by the shared async tests that the library's own
/// <see cref="SystemLinqAsyncAdapter"/> does not need.
/// </summary>
internal static class SystemLinqAsyncTestAdapter
{
    public static ValueTask<bool> AllAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<bool>> predicate, CancellationToken cancellationToken = default)
        => source.AllAwaitWithCancellationAsync(predicate, cancellationToken);
}
