using Funcky.Internal;

namespace Funcky.Async.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <summary>Partitions the result values in an <see cref="IAsyncEnumerable{T}"/> into an error and ok partition.</summary>
    public static ValueTask<ResultPartitions<TValidResult>> PartitionAsync<TValidResult>(
        this IAsyncEnumerable<Result<TValidResult>> source,
        CancellationToken cancellationToken = default)
        => source.PartitionAsync((error, ok) => new ResultPartitions<TValidResult>(error, ok), cancellationToken);

    /// <summary>Partitions the either values in an <see cref="IEnumerable{T}"/> into an error and ok partition.</summary>
    public static async ValueTask<TResult> PartitionAsync<TValidResult, TResult>(
        this IAsyncEnumerable<Result<TValidResult>> source,
        Func<IReadOnlyList<Exception>, IReadOnlyList<TValidResult>, TResult> resultSelector,
        CancellationToken cancellationToken = default)
        => (await source
            .AggregateAsync(new PartitionBuilder<Exception, TValidResult>(), PartitionBuilder.Add, cancellationToken)
            .ConfigureAwait(false))
            .Build(resultSelector);
}
