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
    extension<TSource>(IAsyncEnumerable<TSource> source)
    {
        [Pure]
        public IAsyncEnumerable<TSource> Where(Func<TSource, CancellationToken, ValueTask<bool>> predicate)
            => source.WhereAwaitWithCancellation(predicate);

        [Pure]
        public IAsyncEnumerable<TResult> Select<TResult>(Func<TSource, CancellationToken, ValueTask<TResult>> selector)
            => source.SelectAwaitWithCancellation(selector);

        [Pure]
        public IAsyncEnumerable<TResult> Select<TResult>(Func<TSource, int, CancellationToken, ValueTask<TResult>> selector)
            => source.SelectAwaitWithCancellation(selector);

        [Pure]
        public ValueTask<bool> AnyAsync(Func<TSource, CancellationToken, ValueTask<bool>> predicate, CancellationToken cancellationToken = default)
            => source.AnyAwaitWithCancellationAsync(predicate, cancellationToken);

        [Pure]
        public ValueTask<TAccumulate> AggregateAsync<TAccumulate>(TAccumulate seed, Func<TAccumulate, TSource, CancellationToken, ValueTask<TAccumulate>> accumulator, CancellationToken cancellationToken = default)
            => source.AggregateAwaitWithCancellationAsync(seed, accumulator, cancellationToken);
    }
}
#endif
