namespace Funcky;

public static partial class AsyncSequence
{
    [Pure]
    public static IAsyncEnumerable<TItem> Return<TItem>(TItem item)
        => Return(items: item);

    [Pure]
    public static IAsyncEnumerable<TItem> Return<TItem>(params TItem[] items)
        => items.ToAsyncEnumerable();
}
