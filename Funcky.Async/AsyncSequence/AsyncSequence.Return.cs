namespace Funcky;

public static partial class AsyncSequence
{
    [Pure]
    public static IAsyncEnumerable<TResult> Return<TResult>(TResult element)
        => Return(elements: element);

    [Pure]
    public static IAsyncEnumerable<TResult> Return<TResult>(params TResult[] elements)
        => elements.ToAsyncEnumerable();
}
