using Funcky.Internal;

namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    /// <summary>Partitions the either values in an <see cref="IEnumerable{T}"/> into a left and a right partition.</summary>
    public static EitherPartitions<TLeft, TRight> Partition<TLeft, TRight>(this IEnumerable<Either<TLeft, TRight>> source)
        => source.Partition(EitherPartitions.Create);

    /// <inheritdoc cref="Partition{TLeft,TRight}(IEnumerable{Either{TLeft,TRight}})"/>
    public static TResult Partition<TLeft, TRight, TResult>(this IEnumerable<Either<TLeft, TRight>> source, Func<IReadOnlyList<TLeft>, IReadOnlyList<TRight>, TResult> resultSelector)
        => source
            .Aggregate(new PartitionBuilder<TLeft, TRight>(), PartitionBuilder.Add)
            .Build(resultSelector);

    /// <summary>Partitions the values in an <see cref="IEnumerable{T}"/> into a left and a right partition.</summary>
    public static EitherPartitions<TLeft, TRight> Partition<TSource, TLeft, TRight>(
        this IEnumerable<TSource> source,
        Func<TSource, Either<TLeft, TRight>> selector)
        => source.Select(selector).Partition();

    /// <inheritdoc cref="Partition{TSource,TLeft,TRight}(IEnumerable{TSource}, Func{TSource, Either{TLeft,TRight}})"/>
    public static TResult Partition<TSource, TLeft, TRight, TResult>(
        this IEnumerable<TSource> source,
        Func<TSource, Either<TLeft, TRight>> selector,
        Func<IReadOnlyList<TLeft>, IReadOnlyList<TRight>, TResult> resultSelector)
        => source.Select(selector).Partition(resultSelector);
}
