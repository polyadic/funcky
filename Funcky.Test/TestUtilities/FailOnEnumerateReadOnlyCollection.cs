using System.Collections;
using Xunit.Sdk;

namespace Funcky.Test.TestUtilities;

internal record FailOnEnumerateReadOnlyCollection<T>(int Count) : IReadOnlyCollection<T>
{
    public IEnumerator<T> GetEnumerator() => throw new XunitException("Should not be enumerated");

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
