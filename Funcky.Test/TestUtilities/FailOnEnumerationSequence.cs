using System.Collections;
using Xunit.Sdk;

namespace Funcky.Test.TestUtilities;

internal sealed class FailOnEnumerationSequence<T> : IEnumerable<T>
{
    // Throws from GetEnumerator itself (not from the first MoveNext), so that an operator which acquires the
    // enumerator eagerly is caught by the laziness tests.
    public IEnumerator<T> GetEnumerator()
        => throw new XunitException("Sequence was unexpectedly enumerated.");

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}
