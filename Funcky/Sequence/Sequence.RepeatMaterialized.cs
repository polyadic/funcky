namespace Funcky;

public static partial class Sequence
{
    /// <inheritdoc cref="EnumerableExtensions.extension(Enumerable).RepeatMaterialized{TSource}(IReadOnlyCollection{TSource}, int)"/>
    [Pure]
    public static IEnumerable<TSource> RepeatMaterialized<TSource>(IReadOnlyCollection<TSource> source, int count)
        => Enumerable.RepeatMaterialized(source, count);
}
