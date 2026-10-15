using System.Collections.Immutable;
using Funcky.Internal.Validators;

namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    /// <summary>
    /// Chunks the source sequence into equally sized chunks. The last chunk can be smaller.
    /// </summary>
    /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="size">The desired size of the chunks.</param>
    /// <remarks>On .NET 6 and later the extension method form binds to the BCL's <c>Enumerable.Chunk</c>, which yields arrays; this overload is then only callable as a static method and keeps the <see cref="IReadOnlyList{T}"/> result. On older frameworks it is the extension method, so the element type of the chunks differs per target framework.</remarks>
    /// <returns>A sequence of equally sized sequences containing elements of the source collection in the same order.</returns>
    [Pure]
#if NET6_0_OR_GREATER
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static IEnumerable<IReadOnlyList<TSource>> Chunk<TSource>(IEnumerable<TSource> source, int size)
#else
    public static IEnumerable<IReadOnlyList<TSource>> Chunk<TSource>(this IEnumerable<TSource> source, int size)
#endif
        => ChunkEnumerable(ArgumentValidator.NotNull(source), ChunkSizeValidator.Validate(size));

    /// <summary>
    /// Chunks the source sequence into equally sized chunks. The last chunk can be smaller.
    /// </summary>
    /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    /// <typeparam name="TResult">Type of the elements returned.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="size">The desired size of the chunks.</param>
    /// <param name="resultSelector">The result selector will be applied on each chunked sequence and can produce a desired result.</param>
    /// <returns>A sequence of results based on equally sized chunks.</returns>
    [Pure]
    public static IEnumerable<TResult> Chunk<TSource, TResult>(this IEnumerable<TSource> source, int size, Func<IReadOnlyList<TSource>, TResult> resultSelector)
        => ChunkEnumerable(ArgumentValidator.NotNull(source), ChunkSizeValidator.Validate(size))
            .Select(ArgumentValidator.NotNull(resultSelector));

    private static IEnumerable<IReadOnlyList<TSource>> ChunkEnumerable<TSource>(IEnumerable<TSource> source, int size)
    {
        using var sourceEnumerator = source.GetEnumerator();

        while (sourceEnumerator.MoveNext())
        {
            yield return TakeSkip(sourceEnumerator, size).ToImmutableList();
        }
    }

    private static IEnumerable<TSource> TakeSkip<TSource>(IEnumerator<TSource> source, int size)
    {
        do
        {
            yield return source.Current;
        }
        while (--size > 0 && source.MoveNext());
    }
}
