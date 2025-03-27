using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Funcky.Buffers;

internal static class CollectionBuffer
{
    public static CollectionBuffer<T> Create<T>(ICollection<T> list) => new(list);
}

[SuppressMessage("IDisposableAnalyzers.Correctness", "IDISP025:Class with no virtual dispose method should be sealed", Justification = "Dispose doesn't do anything except flag object as disposed")]
internal class CollectionBuffer<T>(ICollection<T> source) : IBuffer<T>, ICollection<T>
{
    private bool _disposed;

    public int Count
    {
        get
        {
            ThrowIfDisposed();
            return source.Count;
        }
    }

    public bool IsReadOnly
    {
        get
        {
            ThrowIfDisposed();
            return source.IsReadOnly;
        }
    }

    public void Dispose()
    {
        _disposed = true;
    }

    public IEnumerator<T> GetEnumerator()
    {
        ThrowIfDisposed();
        return source.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(T item)
    {
        ThrowIfDisposed();
        source.Add(item);
    }

    public void Clear()
    {
        ThrowIfDisposed();
        source.Clear();
    }

    public bool Contains(T item)
    {
        ThrowIfDisposed();
        return source.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        ThrowIfDisposed();
        source.CopyTo(array, arrayIndex);
    }

    public bool Remove(T item)
    {
        ThrowIfDisposed();
        return source.Remove(item);
    }

    protected void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(ListBuffer<T>));
        }
    }
}
