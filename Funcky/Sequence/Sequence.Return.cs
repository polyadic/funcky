using System.Diagnostics.CodeAnalysis;

namespace Funcky
{
    public static partial class Sequence
    {
        [Pure]
        [SuppressMessage("Funcky", "λ1001:Use of Enumerable.Repeat for a single element", Justification = "This is the implementation of Sequence.Return")]
        public static IEnumerable<TItem> Return<TItem>(TItem item)
            => Enumerable.Repeat(item, 1);

        [Pure]
        public static IEnumerable<TItem> Return<TItem>(params TItem[] items)
            => items;
    }
}
