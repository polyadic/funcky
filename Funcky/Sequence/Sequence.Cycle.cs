namespace Funcky;

public static partial class Sequence
{
    /// <inheritdoc cref="EnumerableExtensions.extension(Enumerable).Cycle{TResult}(TResult)"/>
    [Pure]
    public static IEnumerable<TResult> Cycle<TResult>(TResult element)
        => Enumerable.Cycle(element);
}
