using System.Collections;
using Xunit.Sdk;

namespace Funcky.Test.TestUtils
{
    internal sealed class FailOnEnumerationSequence<T> : IEnumerable<T>
    {
        public IEnumerator<T> GetEnumerator()
            => throw new XunitException("Sequence was unexpectedly enumerated.");

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
