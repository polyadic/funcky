using System.Collections;
using Xunit.Sdk;

namespace Funcky.Test.TestUtilities;

internal class FailOnEnumerateCollectionWrapper<T>(ICollection<T> collection) : ICollection<T>
{
    public bool IsReadOnly => true;

    public int Count => collection.Count;

    public IEnumerator<T> GetEnumerator() => throw new XunitException("Should not be enumerated");

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(T item) => throw new NotSupportedException();

    public void Clear() => throw new NotSupportedException();

    public bool Contains(T item) => collection.Contains(item);

    public void CopyTo(T[] array, int arrayIndex) => collection.CopyTo(array, arrayIndex);

    public bool Remove(T item) => throw new NotSupportedException();
}
