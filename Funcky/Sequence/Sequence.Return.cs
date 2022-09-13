namespace Funcky;

public static partial class Sequence
{
    [Pure]
    public static IReadOnlyList<TResult> Return<TResult>(TResult element)
        => Return(elements: element);

    [Pure]
    public static IReadOnlyList<TResult> Return<TResult>(params TResult[] elements)
        => elements;
}
