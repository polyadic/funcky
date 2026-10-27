namespace Funcky;

public static partial class Sequence
{
    /// <inheritdoc cref="EnumerableExtensions.extension(Enumerable).Return{TResult}(TResult)"/>
    [Pure]
    public static IReadOnlyList<TResult> Return<TResult>(TResult element)
        => Enumerable.Return(element);

    /// <inheritdoc cref="EnumerableExtensions.extension(Enumerable).Return{TResult}(TResult[])"/>
    [Pure]
    public static IReadOnlyList<TResult> Return<TResult>(params TResult[] elements)
        => Enumerable.Return(elements);
}
