using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Funcky
{
    public static partial class Sequence
    {
        [Pure]
        public static IEnumerable<TItem> Return<TItem>(TItem item)
            => Enumerable.Repeat(item, 1);
    }
}
