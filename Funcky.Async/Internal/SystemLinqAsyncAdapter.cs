#if SYSTEM_LINQ_ASYNC
namespace Funcky.Extensions;

/// <summary>
/// Funcky's async sources are written against the <c>IAsyncEnumerable</c> LINQ operators that ship with .NET 10.
/// On older frameworks the same sources are compiled against System.Linq.Async, which names the operators
/// taking asynchronous callbacks differently (<c>SelectAwaitWithCancellation</c>, <c>WhereAwaitWithCancellation</c>, …).
/// These adapters provide the .NET 10 shapes on top of System.Linq.Async so the shared sources need no conditional compilation.
/// </summary>
internal static class SystemLinqAsyncAdapter
{
    [Pure]
    public static IAsyncEnumerable<TSource> Where<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<bool>> predicate)
        => source.WhereAwaitWithCancellation(predicate);

    [Pure]
    public static IAsyncEnumerable<TResult> Select<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<TResult>> selector)
        => source.SelectAwaitWithCancellation(selector);

    [Pure]
    public static IAsyncEnumerable<TResult> Select<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, int, CancellationToken, ValueTask<TResult>> selector)
        => source.SelectAwaitWithCancellation(selector);

    [Pure]
    public static ValueTask<bool> AnyAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<bool>> predicate, CancellationToken cancellationToken = default)
        => source.AnyAwaitWithCancellationAsync(predicate, cancellationToken);

    [Pure]
    public static ValueTask<TAccumulate> AggregateAsync<TSource, TAccumulate>(this IAsyncEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, CancellationToken, ValueTask<TAccumulate>> accumulator, CancellationToken cancellationToken = default)
        => source.AggregateAwaitWithCancellationAsync(seed, accumulator, cancellationToken);
}
#endif
