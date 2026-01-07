namespace Funcky;

public static partial class AsyncSequence
{
    /// <summary>
    /// Generates a sequence that contains the same sequence of elements the given number of times.
    /// </summary>
    /// <typeparam name="TSource">Type of the elements to be repeated.</typeparam>
    /// <param name="source">The sequence of elements to be repeated.</param>
    /// <param name="count">The number of times to repeat the value in the generated sequence.</param>
    /// <returns>Returns an infinite IEnumerable cycling through the same elements.</returns>
    [Pure]
    public static IAsyncBuffer<TSource> RepeatRange<TSource>(IAsyncEnumerable<TSource> source, int count)
        => CycleBuffer.Create(source, count);
}
