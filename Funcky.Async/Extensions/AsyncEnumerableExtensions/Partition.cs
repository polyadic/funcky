using Funcky.Internal;
using static Funcky.Async.ValueTaskFactory;

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
    public static ValueTask<Partitions<TItem>> PartitionAsync<TItem>(
        this IAsyncEnumerable<TItem> source,
        Func<TItem, bool> predicate,
        CancellationToken cancellationToken = default)
        => source.PartitionAsync(predicate, Partitions.Create, cancellationToken);

    /// <inheritdoc cref="PartitionAsync{TItem}(System.Collections.Generic.IAsyncEnumerable{TItem},System.Func{TItem,bool},System.Threading.CancellationToken)" />
    public static ValueTask<Partitions<TItem>> PartitionAwaitAsync<TItem>(
        this IAsyncEnumerable<TItem> source,
        Func<TItem, ValueTask<bool>> predicate,
        CancellationToken cancellationToken = default)
        => source.PartitionAwaitAsync(predicate, static (left, right) => ValueTaskFromResult(Partitions.Create(left, right)), cancellationToken);

    /// <inheritdoc cref="PartitionAsync{TItem}(System.Collections.Generic.IAsyncEnumerable{TItem},System.Func{TItem,bool},System.Threading.CancellationToken)" />
    public static ValueTask<Partitions<TItem>> PartitionAwaitWithCancellationAsync<TItem>(
        this IAsyncEnumerable<TItem> source,
        Func<TItem, CancellationToken, ValueTask<bool>> predicate,
        CancellationToken cancellationToken = default)
        => source.PartitionAwaitWithCancellationAsync(predicate, static (left, right, _) => ValueTaskFromResult(Partitions.Create(left, right)), cancellationToken);

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

    /// <inheritdoc cref="PartitionAsync{TItem,TResult}(IAsyncEnumerable{TItem},Func{TItem,bool},Func{IReadOnlyList{TItem},IReadOnlyList{TItem},TResult},System.Threading.CancellationToken)" />
    public static async ValueTask<TResult> PartitionAwaitAsync<TItem, TResult>(
        this IAsyncEnumerable<TItem> source,
        Func<TItem, ValueTask<bool>> predicate,
        Func<IReadOnlyList<TItem>, IReadOnlyList<TItem>, ValueTask<TResult>> resultSelector,
        CancellationToken cancellationToken = default)
    {
        var (left, right) =
            (await source
                .AggregateAwaitAsync(new PartitionBuilder<TItem, TItem>(), AddAwaitAsync(predicate), cancellationToken)
                .ConfigureAwait(false))
                .Build(ValueTuple.Create);
        return await resultSelector(left, right).ConfigureAwait(false);
    }

    /// <inheritdoc cref="PartitionAsync{TItem,TResult}(IAsyncEnumerable{TItem},Func{TItem,bool},Func{IReadOnlyList{TItem},IReadOnlyList{TItem},TResult},System.Threading.CancellationToken)" />
    public static async ValueTask<TResult> PartitionAwaitWithCancellationAsync<TItem, TResult>(
        this IAsyncEnumerable<TItem> source,
        Func<TItem, CancellationToken, ValueTask<bool>> predicate,
        Func<IReadOnlyList<TItem>, IReadOnlyList<TItem>, CancellationToken, ValueTask<TResult>> resultSelector,
        CancellationToken cancellationToken = default)
    {
        var (left, right) =
            (await source
                .AggregateAwaitWithCancellationAsync(new PartitionBuilder<TItem, TItem>(), AddAwaitWithCancellationAsync(predicate), cancellationToken)
                .ConfigureAwait(false))
                .Build(ValueTuple.Create);
        return await resultSelector(left, right, cancellationToken).ConfigureAwait(false);
    }

    private static Func<PartitionBuilder<TSource, TSource>, TSource, ValueTask<PartitionBuilder<TSource, TSource>>> AddAwaitAsync<TSource>(
        Func<TSource, ValueTask<bool>> predicate)
        => async (builder, element) => await predicate(element).ConfigureAwait(false) ? builder.AddLeft(element) : builder.AddRight(element);

    private static Func<PartitionBuilder<TSource, TSource>, TSource, CancellationToken, ValueTask<PartitionBuilder<TSource, TSource>>> AddAwaitWithCancellationAsync<TSource>(
        Func<TSource, CancellationToken, ValueTask<bool>> predicate)
        => async (builder, element, cancellationToken) => await predicate(element, cancellationToken).ConfigureAwait(false) ? builder.AddLeft(element) : builder.AddRight(element);
}
