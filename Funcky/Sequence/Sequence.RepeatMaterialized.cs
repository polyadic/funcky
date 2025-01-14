namespace Funcky;

public static partial class Sequence
{
    /// <inheritdoc cref="RepeatRange{TSource}(IEnumerable{TSource},int)"/>
    [Pure]
    public static IEnumerable<TSource> RepeatMaterialized<TSource>(IReadOnlyCollection<TSource> source, int count)
        => Enumerable.Repeat(source, count).SelectMany(Identity);
}
