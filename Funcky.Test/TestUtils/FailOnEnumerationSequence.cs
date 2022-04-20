using System.Collections;
using Xunit.Sdk;

namespace Funcky.Test.TestUtils
{
    internal sealed class FailOnEnumerationSequence<T> : IEnumerable<T>
    {
        public IEnumerator<T> GetEnumerator()
        {
            throw new XunitException("Sequence was unexpectedly enumerated.");

#pragma warning disable CS0162 // Unreachable code detected
            yield break; // this forces the method emit the statemachine even though it is not reachable!
#pragma warning restore CS0162 // Unreachable code detected
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
