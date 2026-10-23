using Funcky.Internal;

namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    extension<TValidResult>(IEnumerable<Result<TValidResult>> source)
        where TValidResult : notnull
    {
        /// <summary>Partitions the result values in an <see cref="IEnumerable{T}"/> into an error and ok partition.</summary>
        public ResultPartitions<TValidResult> Partition()
            => source.Partition(ResultPartitions.Create);

        /// <summary>Partitions the either values in an <see cref="IEnumerable{T}"/> into an error and ok partition.</summary>
        public TResult Partition<TResult>(Func<IReadOnlyList<Exception>, IReadOnlyList<TValidResult>, TResult> resultSelector)
            => source
                .Aggregate(new PartitionBuilder<Exception, TValidResult>(), PartitionBuilder.Add)
                .Build(resultSelector);
    }
}
