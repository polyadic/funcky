namespace Funcky;

public static partial class Sequence
{
    /// <inheritdoc cref="EnumerableExtensions.extension(Enumerable).Concat{TSource}(IEnumerable{TSource}[])"/>
    [Pure]
    public static IEnumerable<TSource> Concat<TSource>(params IEnumerable<TSource>[] sources)
        => Enumerable.Concat(sources.AsEnumerable());

    /// <inheritdoc cref="EnumerableExtensions.extension(Enumerable).Concat{TSource}(IEnumerable{IEnumerable{TSource}})"/>
    [Pure]
    public static IEnumerable<TSource> Concat<TSource>(IEnumerable<IEnumerable<TSource>> sources)
        => Enumerable.Concat(sources);
}
