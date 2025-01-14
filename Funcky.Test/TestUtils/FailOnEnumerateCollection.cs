using System.Collections;
using Xunit.Sdk;

namespace Funcky.Test.TestUtils;

internal record FailOnEnumerateCollection<T>(int Count) : ICollection<T>, IReadOnlyCollection<T>
{
    public bool IsReadOnly => true;

    public IEnumerator<T> GetEnumerator() => throw new XunitException("Should not be enumerated");

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(T item) => throw new NotSupportedException();

    public void Clear() => throw new NotSupportedException();

    public bool Contains(T item) => throw new NotSupportedException();

    public void CopyTo(T[] array, int arrayIndex) => throw new NotSupportedException();

    public bool Remove(T item) => throw new NotSupportedException();
}
