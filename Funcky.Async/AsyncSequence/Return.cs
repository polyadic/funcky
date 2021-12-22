namespace Funcky
{
    public static partial class AsyncSequence
    {
        [Pure]
        public static IAsyncEnumerable<TItem> Return<TItem>(TItem item)
            => AsyncEnumerable.Repeat(item, 1);

        public static IAsyncEnumerable<TItem> Return<TItem>(params TItem[] items)
            => items.ToAsyncEnumerable();
    }
}
