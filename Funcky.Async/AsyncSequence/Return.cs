using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Funcky
{
    public static partial class AsyncSequence
    {
        [Pure]
        public static IAsyncEnumerable<TItem> Return<TItem>(TItem item)
            => AsyncEnumerable.Repeat(item, 1);
    }
}
