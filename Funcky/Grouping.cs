using System.Collections;
using System.Collections.Immutable;

namespace Funcky;

internal class Grouping<TKey, TElement>(TKey key, IImmutableList<TElement> elements) : IGrouping<TKey, TElement>, IReadOnlyList<TElement>, IList<TElement>
{
    public TKey Key => key;

    public int Count => elements.Count;

    public bool IsReadOnly => true;

    public TElement this[int index]
    {
        get => elements[index];
        set => throw new NotSupportedException();
    }

    public void Add(TElement item)
        => throw new NotSupportedException();

    public void Clear()
        => throw new NotSupportedException();

    public bool Contains(TElement element)
        => elements.Contains(element);

    public void CopyTo(TElement[] array, int arrayIndex)
        => throw new NotSupportedException();

    public IEnumerator<TElement> GetEnumerator()
        => elements.GetEnumerator();

    public int IndexOf(TElement element)
        => elements.IndexOf(element);

    public void Insert(int index, TElement element)
        => throw new NotSupportedException();

    public bool Remove(TElement element)
        => throw new NotSupportedException();

    public void RemoveAt(int index)
        => throw new NotSupportedException();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}
