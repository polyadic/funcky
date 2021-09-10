#if RANGE_SUPPORTED
using System.Collections;

namespace Funcky.Internal
{
    internal class RangeEnumerable : IEnumerable<int>
    {
        private readonly Range _range;

        public RangeEnumerable(Range range)
            => _range = range;

        public IEnumerator<int> GetEnumerator()
            => _range.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
#endif
