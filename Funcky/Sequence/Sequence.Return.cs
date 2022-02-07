namespace Funcky
{
    public static partial class Sequence
    {
        [Pure]
        public static IReadOnlyList<TItem> Return<TItem>(TItem item)
            => Return(items: item);

        [Pure]
        public static IReadOnlyList<TItem> Return<TItem>(params TItem[] items)
            => items;
    }
}
