using System.Collections.Generic;

namespace Funcky
{
    public static partial class Sequence
    {
        public static IEnumerable<TItem> Cycle<TItem>(TItem element, int count)
            where TItem : notnull
            => Generate(element, Functional.Identity);
    }
}
