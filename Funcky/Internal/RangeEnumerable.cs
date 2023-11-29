#if RANGE_SUPPORTED
using System.Collections;

namespace Funcky.Internal;

internal class RangeEnumerable(Range range) : IEnumerable<int>
{
    public IEnumerator<int> GetEnumerator()
        => range.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}
#endif
