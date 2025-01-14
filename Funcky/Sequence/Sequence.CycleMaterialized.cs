namespace Funcky;

public static partial class Sequence
{
    /// <inheritdoc cref="CycleRange{TSource}(IEnumerable{TSource})"/>
    [Pure]
    public static IEnumerable<TSource> CycleMaterialized<TSource>(IReadOnlyCollection<TSource> source)
        => source.Count > 0
            ? Cycle(source).SelectMany(Identity)
            : throw new InvalidOperationException("you cannot cycle an empty enumerable");
}
