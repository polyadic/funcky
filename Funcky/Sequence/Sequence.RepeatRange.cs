namespace Funcky;

public static partial class Sequence
{
    /// <summary>
    /// Generates a sequence that contains the same sequence of elements the given number of times.
    /// </summary>
    /// <typeparam name="TSource">Type of the elements to be repeated.</typeparam>
    /// <param name="source">The sequence of elements to be repeated.</param>
    /// <param name="count">The number of times to repeat the value in the generated sequence.</param>
    /// <returns>Returns an infinite IEnumerable cycling through the same elements.</returns>
    /// <remarks>Use <see cref="RepeatMaterialized{TSource}"/> if you need to cycle an already materialized sequence.</remarks>
    [Pure]
    public static IBuffer<TSource> RepeatRange<TSource>(IEnumerable<TSource> source, int count)
        => CycleBuffer.Create(source, count);
}
