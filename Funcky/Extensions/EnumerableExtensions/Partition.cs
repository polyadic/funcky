using Funcky.Internal;

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
    public static Partitions<TSource> Partition<TSource>(
        this IEnumerable<TSource> source,
        Func<TSource, bool> predicate)
        => source.Partition(predicate, Partitions.Create);

    /// <summary>
    /// Partitions the items in an <see cref="IEnumerable{T}"/> by the given <paramref name="predicate"/>.
    /// The <paramref name="resultSelector"/> receives the items for which the predicate holds and the items
    /// for which it doesn't as separate parameters.
    /// </summary>
    /// <remarks>This method causes the items in <paramref name="source"/> to be materialized.</remarks>
    public static TResult Partition<TSource, TResult>(
        this IEnumerable<TSource> source,
        Func<TSource, bool> predicate,
        Func<IReadOnlyList<TSource>, IReadOnlyList<TSource>, TResult> resultSelector)
        => source
            .Aggregate(new PartitionBuilder<TSource, TSource>(), PartitionBuilder.Add(predicate))
            .Build(resultSelector);
}
