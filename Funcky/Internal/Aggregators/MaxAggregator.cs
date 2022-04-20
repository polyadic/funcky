namespace Funcky.Internal.Aggregators;

internal static class MaxAggregator
{
    public static Option<TResult> Aggregate<TResult>(Option<TResult> min, TResult current)
        where TResult : notnull
        => min.Match(none: current, some: Maximum(current));

    private static Func<TSource, TSource> Maximum<TSource>(TSource right)
        => left
            => Comparer<TSource>.Default.Compare(left, right) > 0
                ? left
                : right;
}
