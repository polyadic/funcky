using Funcky.Internal;

namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    extension<TLeft, TRight>(IEnumerable<Either<TLeft, TRight>> source)
        where TLeft : notnull
        where TRight : notnull
    {
        /// <summary>Partitions the either values in an <see cref="IEnumerable{T}"/> into a left and a right partition.</summary>
        public EitherPartitions<TLeft, TRight> Partition()
            => source.Partition(EitherPartitions.Create);

        /// <inheritdoc cref="Partition{TLeft,TRight}(IEnumerable{Either{TLeft,TRight}})"/>
        public TResult Partition<TResult>(Func<IReadOnlyList<TLeft>, IReadOnlyList<TRight>, TResult> resultSelector)
            => source
                .Aggregate(new PartitionBuilder<TLeft, TRight>(), PartitionBuilder.Add)
                .Build(resultSelector);
    }

    extension<TSource>(IEnumerable<TSource> source)
    {
        /// <summary>Partitions the values in an <see cref="IEnumerable{T}"/> into a left and a right partition.</summary>
        public EitherPartitions<TLeft, TRight> Partition<TLeft, TRight>(
            Func<TSource, Either<TLeft, TRight>> selector)
            where TLeft : notnull
            where TRight : notnull
            => source.Select(selector).Partition();

        /// <inheritdoc cref="Partition{TSource,TLeft,TRight}(IEnumerable{TSource}, Func{TSource, Either{TLeft,TRight}})"/>
        public TResult Partition<TLeft, TRight, TResult>(
            Func<TSource, Either<TLeft, TRight>> selector,
            Func<IReadOnlyList<TLeft>, IReadOnlyList<TRight>, TResult> resultSelector)
            where TLeft : notnull
            where TRight : notnull
            => source.Select(selector).Partition(resultSelector);
    }
}
