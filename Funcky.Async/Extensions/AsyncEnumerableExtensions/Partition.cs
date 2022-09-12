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
    public static ValueTask<Partitions<TSource>> PartitionAsync<TSource>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, bool> predicate,
        CancellationToken cancellationToken = default)
        => source.PartitionAsync(predicate, Partitions.Create, cancellationToken);

    /// <inheritdoc cref="PartitionAsync{TSource}(System.Collections.Generic.IAsyncEnumerable{TSource},System.Func{TSource,bool},System.Threading.CancellationToken)" />
    public static ValueTask<Partitions<TSource>> PartitionAwaitAsync<TSource>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, ValueTask<bool>> predicate,
        CancellationToken cancellationToken = default)
        => source.PartitionAwaitAsync(predicate, static (left, right) => ValueTaskFromResult(Partitions.Create(left, right)), cancellationToken);

    /// <inheritdoc cref="PartitionAsync{TSource}(System.Collections.Generic.IAsyncEnumerable{TSource},System.Func{TSource,bool},System.Threading.CancellationToken)" />
    public static ValueTask<Partitions<TSource>> PartitionAwaitWithCancellationAsync<TSource>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, CancellationToken, ValueTask<bool>> predicate,
        CancellationToken cancellationToken = default)
        => source.PartitionAwaitWithCancellationAsync(predicate, static (left, right, _) => ValueTaskFromResult(Partitions.Create(left, right)), cancellationToken);

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

    /// <inheritdoc cref="PartitionAsync{TSource,TResult}(IAsyncEnumerable{TSource},Func{TSource,bool},Func{IReadOnlyList{TSource},IReadOnlyList{TSource},TResult},System.Threading.CancellationToken)" />
    public static async ValueTask<TResult> PartitionAwaitAsync<TSource, TResult>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, ValueTask<bool>> predicate,
        Func<IReadOnlyList<TSource>, IReadOnlyList<TSource>, ValueTask<TResult>> resultSelector,
        CancellationToken cancellationToken = default)
    {
        var (left, right) =
            (await source
                .AggregateAwaitAsync(new PartitionBuilder<TSource, TSource>(), AddAwaitAsync(predicate), cancellationToken)
                .ConfigureAwait(false))
                .Build(ValueTuple.Create);
        return await resultSelector(left, right).ConfigureAwait(false);
    }

    /// <inheritdoc cref="PartitionAsync{TSource,TResult}(IAsyncEnumerable{TSource},Func{TSource,bool},Func{IReadOnlyList{TSource},IReadOnlyList{TSource},TResult},System.Threading.CancellationToken)" />
    public static async ValueTask<TResult> PartitionAwaitWithCancellationAsync<TSource, TResult>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, CancellationToken, ValueTask<bool>> predicate,
        Func<IReadOnlyList<TSource>, IReadOnlyList<TSource>, CancellationToken, ValueTask<TResult>> resultSelector,
        CancellationToken cancellationToken = default)
    {
        var (left, right) =
            (await source
                .AggregateAwaitWithCancellationAsync(new PartitionBuilder<TSource, TSource>(), AddAwaitWithCancellationAsync(predicate), cancellationToken)
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
