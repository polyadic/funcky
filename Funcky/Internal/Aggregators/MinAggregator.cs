namespace Funcky.Internal.Aggregators;

internal static class MinAggregator
{
    public static Option<TResult> Aggregate<TResult>(Option<TResult> min, TResult current)
        where TResult : notnull
        => min.Match(none: current, some: Minimum(current));

    // For floats this defines a total order where NaN comes before negative infinity
    private static Func<TSource, TSource> Minimum<TSource>(TSource right)
        => left
            => Comparer<TSource>.Default.Compare(left, right) < 0
                ? left
                : right;
}
