#if INTEGRATED_ASYNC
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
        => sources
            .SelectMany(Identity);

    /// <summary>
    /// Concatenates multiple sequences together.
    /// </summary>
    [Pure]
    public static IAsyncEnumerable<TSource> Concat<TSource>(IEnumerable<IAsyncEnumerable<TSource>> sources)
        => sources.ToAsyncEnumerable()
            .SelectMany(Identity);

    /// <summary>
    /// Concatenates multiple sequences together.
    /// </summary>
    [Pure]
    public static IAsyncEnumerable<TSource> Concat<TSource>(IAsyncEnumerable<IEnumerable<TSource>> sources)
        => sources
            .SelectMany(AsyncEnumerable.ToAsyncEnumerable);
}
#endif
