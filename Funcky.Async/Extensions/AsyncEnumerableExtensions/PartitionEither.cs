using Funcky.Internal;

namespace Funcky.Async.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <summary>Partitions the either values in an <see cref="IAsyncEnumerable{T}"/> into a left and a right partition.</summary>
    public static ValueTask<EitherPartitions<TLeft, TRight>> PartitionAsync<TLeft, TRight>(
        this IAsyncEnumerable<Either<TLeft, TRight>> source,
        CancellationToken cancellationToken = default)
        => source.PartitionAsync((left, right) => new EitherPartitions<TLeft, TRight>(left, right), cancellationToken);

    /// <summary>Partitions the either values in an <see cref="IAsyncEnumerable{T}"/> into a left and a right partition.</summary>
    public static async ValueTask<TResult> PartitionAsync<TLeft, TRight, TResult>(
        this IAsyncEnumerable<Either<TLeft, TRight>> source,
        Func<IReadOnlyList<TLeft>, IReadOnlyList<TRight>, TResult> resultSelector,
        CancellationToken cancellationToken = default)
        => (await source
            .AggregateAsync(new PartitionBuilder<TLeft, TRight>(), PartitionBuilder.Add, cancellationToken)
            .ConfigureAwait(false))
            .Build(resultSelector);
}
