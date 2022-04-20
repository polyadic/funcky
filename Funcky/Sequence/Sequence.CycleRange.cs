using System.Runtime.CompilerServices;

namespace Funcky;

public static partial class Sequence
{
    /// <summary>
    /// Generates a sequence that contains the same sequence of elements over and over again as an endless generator.
    /// </summary>
    /// <typeparam name="TItem">Type of the elements to be cycled.</typeparam>
    /// <param name="sequence">The sequence of elements which are cycled. Throws an exception if the sequence is empty.</param>
    /// <returns>Returns an infinite IEnumerable repeating the same sequence of elements.</returns>
    [Pure]
    public static IEnumerable<TItem> CycleRange<TItem>(IEnumerable<TItem> sequence)
        where TItem : notnull
        => CycleSequenceIfNotEmpty(sequence.Materialize());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static IEnumerable<TItem> CycleSequenceIfNotEmpty<TItem>(IReadOnlyCollection<TItem> sequence)
        where TItem : notnull
        => sequence.Count != 0
            ? ProjectCycles(sequence)
            : throw new ArgumentException("An empty sequence cannot be cycled", nameof(sequence));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static IEnumerable<TItem> ProjectCycles<TItem>(IReadOnlyCollection<TItem> list)
        where TItem : notnull
        => from cycle in Cycle(list)
           from element in cycle
           select element;
}
