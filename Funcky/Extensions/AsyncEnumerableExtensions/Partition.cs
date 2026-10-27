#if ASYNC_SUPPORTED
using Funcky.Internal;
using static Funcky.ValueTaskFactory;

namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <summary>
    /// Partitions the items in an <see cref="IAsyncEnumerable{T}"/> by the given <paramref name="predicate"/>.
    /// <example>
    /// <code><![CDATA[
    /// using System.Linq;
    /// using Funcky.Extensions;
    ///
    /// var (evens, odds) = AsyncEnumerable.Range(0, 6).PartitionAsync(n => n % 2 == 0);
    /// ]]></code>
    /// </example>
    /// </summary>
    /// <remarks>This method causes the items in <paramref name="source"/> to be materialized.</remarks>
    /// <returns>A tuple with the items for which the predicate holds, and for those for which it doesn't.</returns>
    public static ValueTask<Partitions<TSource>> PartitionAsync<TSource>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, bool> predicate,
        CancellationToken cancellationToken = default)
        => source.PartitionAsync(predicate, Partitions.Create, cancellationToken);

    /// <inheritdoc cref="PartitionAsync{TSource}(IAsyncEnumerable{TSource},Func{TSource,bool},CancellationToken)" />
    /// <param name="source">The items to partition.</param>
    /// <param name="predicate">An asynchronous predicate; it receives the cancellation token.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static ValueTask<Partitions<TSource>> PartitionAsync<TSource>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, CancellationToken, ValueTask<bool>> predicate,
        CancellationToken cancellationToken = default)
        => source.PartitionAsync(predicate, static (left, right, _) => ValueTaskFromResult(Partitions.Create(left, right)), cancellationToken);

    /// <inheritdoc cref="PartitionAsync{TSource}(IAsyncEnumerable{TSource},Func{TSource,bool},CancellationToken)" />
    [Obsolete("Use PartitionAsync with a (item, cancellationToken) delegate instead; discard the token with (item, _) => …. This method will be removed in Funcky 4.0.0.")]
    public static ValueTask<Partitions<TSource>> PartitionAwaitAsync<TSource>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, ValueTask<bool>> predicate,
        CancellationToken cancellationToken = default)
        => source.PartitionAsync((item, _) => predicate(item), cancellationToken);

    /// <inheritdoc cref="PartitionAsync{TSource}(IAsyncEnumerable{TSource},Func{TSource,bool},CancellationToken)" />
    [Obsolete("Use PartitionAsync instead, which takes the same delegate. This method will be removed in Funcky 4.0.0.")]
    public static ValueTask<Partitions<TSource>> PartitionAwaitWithCancellationAsync<TSource>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, CancellationToken, ValueTask<bool>> predicate,
        CancellationToken cancellationToken = default)
        => source.PartitionAsync(predicate, cancellationToken);

    /// <summary>
    /// Partitions the items in an <see cref="IAsyncEnumerable{T}"/> by the given <paramref name="predicate"/>.
    /// The <paramref name="resultSelector"/> receives the items for which the predicate holds and the items
    /// for which it doesn't as separate parameters.
    /// </summary>
    /// <remarks>This method causes the items in <paramref name="source"/> to be materialized.</remarks>
    public static async ValueTask<TResult> PartitionAsync<TSource, TResult>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, bool> predicate,
        Func<IReadOnlyList<TSource>, IReadOnlyList<TSource>, TResult> resultSelector,
        CancellationToken cancellationToken = default)
        => (await source
            .AggregateAsync(new PartitionBuilder<TSource, TSource>(), PartitionBuilder.Add(predicate), cancellationToken)
            .ConfigureAwait(false))
            .Build(resultSelector);

    /// <inheritdoc cref="PartitionAsync{TSource,TResult}(IAsyncEnumerable{TSource},Func{TSource,bool},Func{IReadOnlyList{TSource},IReadOnlyList{TSource},TResult},CancellationToken)" />
    /// <param name="source">The items to partition.</param>
    /// <param name="predicate">An asynchronous predicate; it receives the cancellation token.</param>
    /// <param name="resultSelector">An asynchronous result selector; it receives the cancellation token.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static async ValueTask<TResult> PartitionAsync<TSource, TResult>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, CancellationToken, ValueTask<bool>> predicate,
        Func<IReadOnlyList<TSource>, IReadOnlyList<TSource>, CancellationToken, ValueTask<TResult>> resultSelector,
        CancellationToken cancellationToken = default)
    {
        var (left, right) =
            (await source
                .AggregateAsync(new PartitionBuilder<TSource, TSource>(), AddAsync(predicate), cancellationToken)
                .ConfigureAwait(false))
                .Build(ValueTuple.Create);
        return await resultSelector(left, right, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc cref="PartitionAsync{TSource,TResult}(IAsyncEnumerable{TSource},Func{TSource,bool},Func{IReadOnlyList{TSource},IReadOnlyList{TSource},TResult},CancellationToken)" />
    [Obsolete("Use PartitionAsync with (…, cancellationToken) delegates instead; discard the token with (item, _) => …. This method will be removed in Funcky 4.0.0.")]
    public static ValueTask<TResult> PartitionAwaitAsync<TSource, TResult>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, ValueTask<bool>> predicate,
        Func<IReadOnlyList<TSource>, IReadOnlyList<TSource>, ValueTask<TResult>> resultSelector,
        CancellationToken cancellationToken = default)
        => source.PartitionAsync((item, _) => predicate(item), (left, right, _) => resultSelector(left, right), cancellationToken);

    /// <inheritdoc cref="PartitionAsync{TSource,TResult}(IAsyncEnumerable{TSource},Func{TSource,bool},Func{IReadOnlyList{TSource},IReadOnlyList{TSource},TResult},CancellationToken)" />
    [Obsolete("Use PartitionAsync instead, which takes the same delegates. This method will be removed in Funcky 4.0.0.")]
    public static ValueTask<TResult> PartitionAwaitWithCancellationAsync<TSource, TResult>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, CancellationToken, ValueTask<bool>> predicate,
        Func<IReadOnlyList<TSource>, IReadOnlyList<TSource>, CancellationToken, ValueTask<TResult>> resultSelector,
        CancellationToken cancellationToken = default)
        => source.PartitionAsync(predicate, resultSelector, cancellationToken);

    private static Func<PartitionBuilder<TSource, TSource>, TSource, CancellationToken, ValueTask<PartitionBuilder<TSource, TSource>>> AddAsync<TSource>(
        Func<TSource, CancellationToken, ValueTask<bool>> predicate)
        => async (builder, element, cancellationToken) => await predicate(element, cancellationToken).ConfigureAwait(false) ? builder.AddLeft(element) : builder.AddRight(element);
}
#endif
