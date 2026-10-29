namespace Funcky;

public static partial class Sequence
{
    /// <inheritdoc cref="EnumerableExtensions.extension(Enumerable).CycleMaterialized{TSource}(IReadOnlyCollection{TSource})"/>
    [Pure]
    public static IEnumerable<TSource> CycleMaterialized<TSource>(IReadOnlyCollection<TSource> source)
        => Enumerable.CycleMaterialized(source);
}
