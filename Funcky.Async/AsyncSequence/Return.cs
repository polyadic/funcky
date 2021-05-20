using System.Collections.Generic;
using System.Linq;

namespace Funcky
{
    public static partial class AsyncSequence
    {
        public static IAsyncEnumerable<TItem> Return<TItem>(TItem item)
            => AsyncEnumerable.Repeat(item, 1);
    }
}
