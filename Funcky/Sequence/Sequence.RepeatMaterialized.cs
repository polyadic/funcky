namespace Funcky;

public static partial class Sequence
{
    /// <summary>
    /// Generates a sequence that contains the same sequence of elements the given number of times.
    /// </summary>
    /// <typeparam name="TSource">Type of the elements to be repeated.</typeparam>
    /// <param name="source">The sequence of elements to be repeated.</param>
    /// <param name="count">The number of times to repeat the sequence in the generated sequence; 0 yields an empty sequence.</param>
    /// <returns>Returns a finite sequence containing the elements of <paramref name="source"/> <paramref name="count"/> times.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is negative.</exception>
    /// <remarks>Use <see cref="RepeatRange{TSource}"/> if you need to cycle a lazy sequence.</remarks>
    [Pure]
    public static IEnumerable<TSource> RepeatMaterialized<TSource>(IReadOnlyCollection<TSource> source, int count)
        => Enumerable.Repeat(source, count).Flatten();
}
