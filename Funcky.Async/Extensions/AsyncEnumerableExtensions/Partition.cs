using Funcky.Internal;

namespace Funcky.Async.Extensions;

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
    public static ValueTask<(IReadOnlyList<TItem> True, IReadOnlyList<TItem> False)> PartitionAsync<TItem>(
        this IAsyncEnumerable<TItem> source,
        Func<TItem, bool> predicate,
        CancellationToken cancellationToken = default)
        => source.PartitionAsync(predicate, ValueTuple.Create, cancellationToken);

    /// <summary>
    /// Partitions the items in an <see cref="IAsyncEnumerable{T}"/> by the given <paramref name="predicate"/>.
    /// The <paramref name="resultSelector"/> receives the items for which the predicate holds and the items
    /// for which it doesn't as separate parameters.
    /// </summary>
    /// <remarks>This method causes the items in <paramref name="source"/> to be materialized.</remarks>
    public static async ValueTask<TResult> PartitionAsync<TItem, TResult>(
        this IAsyncEnumerable<TItem> source,
        Func<TItem, bool> predicate,
        Func<IReadOnlyList<TItem>, IReadOnlyList<TItem>, TResult> resultSelector,
        CancellationToken cancellationToken = default)
        => (await source
            .AggregateAsync(new PartitionBuilder<TItem, TItem>(), PartitionBuilder.Add(predicate), cancellationToken)
            .ConfigureAwait(false))
            .Build(resultSelector);
}
