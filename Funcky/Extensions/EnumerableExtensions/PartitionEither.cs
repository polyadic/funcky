namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    public static EitherPartitions<TLeft, TRight> Partition<TLeft, TRight>(this IEnumerable<Either<TLeft, TRight>> source)
        => source
            .Aggregate(PartitionBuilder<TLeft, TRight>.Default, Add)
            .Build((left, right) => new EitherPartitions<TLeft, TRight>(left, right));

    private static PartitionBuilder<TLeft, TRight> Add<TLeft, TRight>(PartitionBuilder<TLeft, TRight> builder, Either<TLeft, TRight> either)
        => either.Match(left: builder.AddLeft, right: builder.AddRight);
}
