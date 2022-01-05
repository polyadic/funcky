using System.Collections;
using Xunit.Sdk;

namespace Funcky.Test.TestUtils
{
    internal sealed class FailOnEnumerationSequence<T> : IEnumerable<T>
    {
        public IEnumerator<T> GetEnumerator()
            => new FailOnEnumerationEnumerator<T>();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }

    internal sealed class FailOnEnumerationEnumerator<T> : IEnumerator<T>
    {
        public T Current => throw new NotImplementedException();

        object IEnumerator.Current => throw new NotImplementedException();

        public void Dispose()
        {
        }

        public bool MoveNext()
            => throw new XunitException("Sequence was unexpectedly enumerated.");

        public void Reset()
            => throw new XunitException("Sequence was unexpectedly reset.");
    }
}
