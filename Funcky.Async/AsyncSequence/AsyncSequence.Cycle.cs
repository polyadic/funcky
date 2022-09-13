using static Funcky.Async.ValueTaskFactory;

namespace Funcky;

public static partial class AsyncSequence
{
    /// <summary>
    /// Cycles the same element over and over again as an endless generator.
    /// </summary>
    /// <typeparam name="TResult">Type of the element to be cycled.</typeparam>
    /// <param name="element">The element to be cycled.</param>
    /// <returns>Returns an infinite IEnumerable cycling through the same elements.</returns>
    [Pure]
    public static IAsyncEnumerable<TResult> Cycle<TResult>(TResult element)
        => Successors(element, ValueTaskFromResult);
}
