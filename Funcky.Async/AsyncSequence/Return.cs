using System.Diagnostics.Contracts;

namespace Funcky
{
    public static partial class AsyncSequence
    {
        [Pure]
        public static IAsyncEnumerable<TItem> Return<TItem>(TItem item)
            => AsyncEnumerable.Repeat(item, 1);
    }
}
