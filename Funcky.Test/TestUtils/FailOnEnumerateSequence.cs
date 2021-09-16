using Xunit.Sdk;

namespace Funcky.Test.TestUtils
{
    internal sealed class FailOnEnumerateSequence<T> : IEnumerable<T>
    {
        public IEnumerator<T> GetEnumerator()
            => throw new XunitException("Sequence was unexpectedly enumerated.");

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
