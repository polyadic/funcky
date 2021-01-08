using System.Collections.Generic;
using System.Linq;

namespace Funcky
{
    public static partial class Sequence
    {
        public static IEnumerable<TItem> RepeatRange<TItem>(IEnumerable<TItem> sequence, int count)
            => Enumerable
                .Repeat(Unit.Value, count)
                .SelectMany(_ => sequence);
    }
}
