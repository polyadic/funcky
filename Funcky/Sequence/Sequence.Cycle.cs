namespace Funcky;

public static partial class Sequence
{
    /// <summary>
    /// Cycles the same element over and over again as an endless generator.
    /// </summary>
    /// <typeparam name="TItem">Type of the element to be cycled.</typeparam>
    /// <param name="element">The element to be cycled.</param>
    /// <returns>Returns an infinite IEnumerable cycling through the same elements.</returns>
    [Pure]
    public static IEnumerable<TItem> Cycle<TItem>(TItem element)
        where TItem : notnull
        => Successors(element, Identity);
}
