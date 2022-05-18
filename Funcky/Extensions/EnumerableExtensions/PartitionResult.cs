using Funcky.Internal;

namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    /// <summary>Partitions the result values in an <see cref="IEnumerable{T}"/> into an error and ok partition.</summary>
    public static ResultPartitions<TValidResult> Partition<TValidResult>(this IEnumerable<Result<TValidResult>> source)
        => source.Partition((error, ok) => new ResultPartitions<TValidResult>(error, ok));

    /// <summary>Partitions the either values in an <see cref="IEnumerable{T}"/> into an error and ok partition.</summary>
    public static TResult Partition<TValidResult, TResult>(this IEnumerable<Result<TValidResult>> source, Func<IReadOnlyList<Exception>, IReadOnlyList<TValidResult>, TResult> resultSelector)
        => source
            .Aggregate(new PartitionBuilder<Exception, TValidResult>(), PartitionBuilder.Add)
            .Build(resultSelector);
}
