using System.Collections.Generic;
using System.Linq;

namespace Funcky
{
    public static partial class Sequence
    {
        public static IEnumerable<TItem> CycleRange<TItem>(IEnumerable<TItem> sequence, int count)
            => Generate(Unit.Value, Functional.Identity)
                .SelectMany(_ => sequence);
    }
}
