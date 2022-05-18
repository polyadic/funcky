namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    /// <summary>Partitions the either values in an <see cref="IEnumerable{T}"/> into a left and a right partition.</summary>
    public static EitherPartitions<TLeft, TRight> Partition<TLeft, TRight>(this IEnumerable<Either<TLeft, TRight>> source)
        => source.Partition((left, right) => new EitherPartitions<TLeft, TRight>(left, right));

    /// <summary>Partitions the either values in an <see cref="IEnumerable{T}"/> into a left and a right partition.</summary>
    public static TResult Partition<TLeft, TRight, TResult>(this IEnumerable<Either<TLeft, TRight>> source, Func<IReadOnlyList<TLeft>, IReadOnlyList<TRight>, TResult> resultSelector)
        => source
            .Aggregate(new PartitionBuilder<TLeft, TRight>(), Add)
            .Build(resultSelector);

    private static PartitionBuilder<TLeft, TRight> Add<TLeft, TRight>(PartitionBuilder<TLeft, TRight> builder, Either<TLeft, TRight> either)
        => either.Match(left: builder.AddLeft, right: builder.AddRight);
}
