using System.Collections;
using System.Collections.Immutable;

namespace Funcky;

internal class Grouping<TKey, TItem>(TKey key, IImmutableList<TItem> items) : IGrouping<TKey, TItem>, IReadOnlyList<TItem>, IList<TItem>
{
    public TKey Key => key;

    public int Count => items.Count;

    public bool IsReadOnly => true;

    public TItem this[int index]
    {
        get => items[index];
        set => throw new NotSupportedException();
    }

    public void Add(TItem item)
        => throw new NotSupportedException();

    public void Clear()
        => throw new NotSupportedException();

    public bool Contains(TItem item)
        => items.Contains(item);

    public void CopyTo(TItem[] array, int arrayIndex)
        => throw new NotSupportedException();

    public IEnumerator<TItem> GetEnumerator()
        => items.GetEnumerator();

    public int IndexOf(TItem item)
        => items.IndexOf(item);

    public void Insert(int index, TItem item)
        => throw new NotSupportedException();

    public bool Remove(TItem item)
        => throw new NotSupportedException();

    public void RemoveAt(int index)
        => throw new NotSupportedException();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}
