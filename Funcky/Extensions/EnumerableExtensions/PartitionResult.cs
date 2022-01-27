namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    /// <summary>Partitions the result values in an <see cref="IEnumerable{T}"/> into an ok and an error partition.</summary>
    public static ResultPartitions<TValidResult> Partition<TValidResult>(this IEnumerable<Result<TValidResult>> source)
        => source.Partition((left, right) => new ResultPartitions<TValidResult>(left, right));

    /// <summary>Partitions the either values in an <see cref="IEnumerable{T}"/> into an ok and an error partition.</summary>
    public static TResult Partition<TValidResult, TResult>(this IEnumerable<Result<TValidResult>> source, Func<IReadOnlyCollection<TValidResult>, IReadOnlyCollection<Exception>, TResult> resultSelector)
        => source
            .Aggregate(PartitionBuilder<TValidResult, Exception>.Default, Add)
            .Build(resultSelector);

    private static PartitionBuilder<TValidResult, Exception> Add<TValidResult>(PartitionBuilder<TValidResult, Exception> builder, Result<TValidResult> result)
        => result.Match(ok: builder.AddLeft, error: builder.AddRight);
}
