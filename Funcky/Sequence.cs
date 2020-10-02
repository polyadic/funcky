using System.Collections.Generic;
using System.Linq;

namespace Funcky
{
    public static class Sequence
    {
        public static IEnumerable<TItem> Return<TItem>(TItem item) => Enumerable.Repeat(item, 1);
    }
}
