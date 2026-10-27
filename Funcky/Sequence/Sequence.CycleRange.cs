namespace Funcky;

public static partial class Sequence
{
    /// <inheritdoc cref="EnumerableExtensions.extension(Enumerable).CycleRange{TSource}(IEnumerable{TSource})"/>
    [Pure]
    public static IBuffer<TSource> CycleRange<TSource>(IEnumerable<TSource> source)
        => Enumerable.CycleRange(source);
}
