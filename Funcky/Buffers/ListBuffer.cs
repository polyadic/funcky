namespace Funcky.Buffers;

internal static class ListBuffer
{
    public static ListBuffer<T> Create<T>(IList<T> list) => new(list);
}

internal sealed class ListBuffer<T>(IList<T> source) : CollectionBuffer<T>(source), IList<T>
{
    public T this[int index]
    {
        get
        {
            ThrowIfDisposed();
            return source[index];
        }

        set
        {
            ThrowIfDisposed();
            source[index] = value;
        }
    }

    public int IndexOf(T item)
    {
        ThrowIfDisposed();
        return source.IndexOf(item);
    }

    public void Insert(int index, T item)
    {
        ThrowIfDisposed();
        source.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        ThrowIfDisposed();
        source.RemoveAt(index);
    }
}
