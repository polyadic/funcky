#if INTEGRATED_ASYNC
using Funcky.Internal;

namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <summary>Partitions the either values in an <see cref="IAsyncEnumerable{T}"/> into a left and a right partition.</summary>
    public static ValueTask<EitherPartitions<TLeft, TRight>> PartitionAsync<TLeft, TRight>(
        this IAsyncEnumerable<Either<TLeft, TRight>> source,
        CancellationToken cancellationToken = default)
        where TLeft : notnull
        where TRight : notnull
        => source.PartitionAsync(EitherPartitions.Create, cancellationToken);

    /// <inheritdoc cref="PartitionAsync{TLeft,TRight}(IAsyncEnumerable{Either{TLeft,TRight}},CancellationToken)"/>
    public static async ValueTask<TResult> PartitionAsync<TLeft, TRight, TResult>(
        this IAsyncEnumerable<Either<TLeft, TRight>> source,
        Func<IReadOnlyList<TLeft>, IReadOnlyList<TRight>, TResult> resultSelector,
        CancellationToken cancellationToken = default)
        where TLeft : notnull
        where TRight : notnull
        => (await source
            .AggregateAsync(new PartitionBuilder<TLeft, TRight>(), PartitionBuilder.Add, cancellationToken)
            .ConfigureAwait(false))
            .Build(resultSelector);

    /// Partitions the items in an <see cref="IAsyncEnumerable{T}"/> into a left and a right partition.
    public static ValueTask<EitherPartitions<TLeft, TRight>> PartitionAsync<TSource, TLeft, TRight>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, Either<TLeft, TRight>> selector,
        CancellationToken cancellationToken = default)
        where TLeft : notnull
        where TRight : notnull
        => source.Select(selector).PartitionAsync(EitherPartitions.Create, cancellationToken);

    /// <inheritdoc cref="PartitionAsync{TSource,TLeft,TRight}(IAsyncEnumerable{TSource},Func{TSource,Either{TLeft,TRight}},CancellationToken)"/>
    public static ValueTask<TResult> PartitionAsync<TSource, TLeft, TRight, TResult>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, Either<TLeft, TRight>> selector,
        Func<IReadOnlyList<TLeft>, IReadOnlyList<TRight>, TResult> resultSelector,
        CancellationToken cancellationToken = default)
        where TLeft : notnull
        where TRight : notnull
        => source.Select(selector).PartitionAsync(resultSelector, cancellationToken);
}
#endif
