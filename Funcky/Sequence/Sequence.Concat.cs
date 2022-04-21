namespace Funcky;

public static partial class Sequence
{
    /// <summary>
    /// Concatenates multiple sequences together.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> Concat<TSource>(params IEnumerable<TSource>[] sources)
        => Concat(sources.AsEnumerable());

    /// <summary>
    /// Concatenates multiple sequences together.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> Concat<TSource>(IEnumerable<IEnumerable<TSource>> sources)
        => from source in sources
           from element in source
           select element;
}
