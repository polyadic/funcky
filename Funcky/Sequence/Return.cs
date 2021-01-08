using System.Collections.Generic;
using System.Linq;

namespace Funcky
{
    public static partial class Sequence
    {
        public static IEnumerable<TItem> Return<TItem>(TItem item)
            where TItem : notnull
            => Enumerable.Repeat(item, 1);
    }
}
