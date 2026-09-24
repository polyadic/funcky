namespace Funcky;

public static partial class Sequence
{
    /// <summary>
    /// Generates a sequence that contains the same sequence of elements over and over again as an endless generator.
    /// </summary>
    /// <typeparam name="TSource">Type of the elements to be cycled.</typeparam>
    /// <param name="source">The sequence of elements which are cycled. Throws an exception if the sequence is empty.</param>
    /// <returns>Returns an infinite IEnumerable repeating the same sequence of elements.</returns>
    /// <remarks>Use <see cref="CycleRange{TSource}"/> if you need to cycle a lazy sequence.</remarks>
    [Pure]
    public static IEnumerable<TSource> CycleMaterialized<TSource>(IReadOnlyCollection<TSource> source)
        => source.Count > 0
            ? Cycle(source).SelectMany(Identity)
            : throw new InvalidOperationException("you cannot cycle an empty enumerable");
}
