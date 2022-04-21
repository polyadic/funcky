namespace Funcky;

public static partial class AsyncSequence
{
    /// <summary>
    /// Concatenates multiple sequences together.
    /// </summary>
    [Pure]
    public static IAsyncEnumerable<TSource> Concat<TSource>(params IAsyncEnumerable<TSource>[] sources)
        => Concat(sources.AsEnumerable());

    /// <summary>
    /// Concatenates multiple sequences together.
    /// </summary>
    [Pure]
    public static IAsyncEnumerable<TSource> Concat<TSource>(IAsyncEnumerable<IAsyncEnumerable<TSource>> sources)
        => from source in sources
           from element in source
           select element;

    /// <summary>
    /// Concatenates multiple sequences together.
    /// </summary>
    [Pure]
    public static IAsyncEnumerable<TSource> Concat<TSource>(IEnumerable<IAsyncEnumerable<TSource>> sources)
        => from source in sources.ToAsyncEnumerable()
           from element in source
           select element;

    /// <summary>
    /// Concatenates multiple sequences together.
    /// </summary>
    [Pure]
    public static IAsyncEnumerable<TSource> Concat<TSource>(IAsyncEnumerable<IEnumerable<TSource>> sources)
        => from source in sources
           from element in source.ToAsyncEnumerable()
           select element;
}
