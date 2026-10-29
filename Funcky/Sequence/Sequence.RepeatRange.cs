namespace Funcky;

public static partial class Sequence
{
    /// <inheritdoc cref="EnumerableExtensions.extension(Enumerable).RepeatRange{TSource}(IEnumerable{TSource}, int)"/>
    [Pure]
    public static IBuffer<TSource> RepeatRange<TSource>(IEnumerable<TSource> source, int count)
        => Enumerable.RepeatRange(source, count);
}
