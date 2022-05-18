namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    /// <summary>
    /// Partitions the items in an <see cref="IEnumerable{T}"/> by the given <paramref name="predicate"/>.
    /// <example>
    /// <code><![CDATA[
    /// using System.Linq;
    /// using Funcky.Extensions;
    ///
    /// var (evens, odds) = Enumerable.Range(0, 6).Partition(n => n % 2 == 0);
    /// ]]></code>
    /// </example>
    /// </summary>
    /// <remarks>This method causes the items in <paramref name="source"/> to be materialized.</remarks>
    /// <returns>A tuple with the items for which the predicate holds, and for those for which it doesn't.</returns>
    public static (IReadOnlyList<TItem> True, IReadOnlyList<TItem> False) Partition<TItem>(
        this IEnumerable<TItem> source,
        Func<TItem, bool> predicate)
        => source.Partition(predicate, ValueTuple.Create);

    /// <summary>
    /// Partitions the items in an <see cref="IEnumerable{T}"/> by the given <paramref name="predicate"/>.
    /// The <paramref name="resultSelector"/> receives the items for which the predicate holds and the items
    /// for which it doesn't as separate parameters.
    /// </summary>
    /// <remarks>This method causes the items in <paramref name="source"/> to be materialized.</remarks>
    public static TResult Partition<TItem, TResult>(
        this IEnumerable<TItem> source,
        Func<TItem, bool> predicate,
        Func<IReadOnlyList<TItem>, IReadOnlyList<TItem>, TResult> resultSelector)
        => source
            .Aggregate(PartitionBuilder<TItem, TItem>.Default, PartitionBuilder.Add(predicate))
            .Build(resultSelector);
}
