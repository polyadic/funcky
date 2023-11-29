using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    /// <summary>
    /// Creates a buffer with a view over the source sequence, causing each enumerator to obtain access to all of the
    /// sequence's elements without causing multiple enumerations over the source.
    /// </summary>
    /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <returns>A lazy buffer of the underlying sequence.</returns>
    [Pure]
    public static IBuffer<TSource> Memoize<TSource>(this IEnumerable<TSource> source)
        => source is IBuffer<TSource> buffer
            ? Borrow(buffer)
            : MemoizedBuffer.Create(source);

    [SuppressMessage("IDisposableAnalyzers", "IDISP015: Member should not return created and cached instance.", Justification = "False positive.")]
    private static IBuffer<TSource> Borrow<TSource>(IBuffer<TSource> buffer)
        => buffer as BorrowedBuffer<TSource> ?? new BorrowedBuffer<TSource>(buffer);

    private static class MemoizedBuffer
    {
        public static MemoizedBuffer<TSource> Create<TSource>(IEnumerable<TSource> source)
            => new(source);
    }

    private sealed class BorrowedBuffer<T>(IBuffer<T> inner) : IBuffer<T>
    {
        private bool _disposed;

        public IEnumerator<T> GetEnumerator()
        {
            ThrowIfDisposed();
            return inner.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Dispose() => _disposed = true;

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(BorrowedBuffer<T>));
            }
        }
    }

    private sealed class MemoizedBuffer<T>(IEnumerable<T> source) : IBuffer<T>
    {
        private readonly List<T> _buffer = new();
        private readonly IEnumerator<T> _source = source.GetEnumerator();

        private bool _disposed;

        public IEnumerator<T> GetEnumerator()
        {
            ThrowIfDisposed();

            return GetEnumeratorInternal();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            ThrowIfDisposed();

            return GetEnumeratorInternal();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _source.Dispose();
                _buffer.Clear();
                _disposed = true;
            }
        }

        private IEnumerator<T> GetEnumeratorInternal()
        {
            for (var index = 0; true; index++)
            {
                ThrowIfDisposed();

                if (index == _buffer.Count)
                {
                    if (_source.MoveNext())
                    {
                        _buffer.Add(_source.Current);
                    }
                    else
                    {
                        break;
                    }
                }

                yield return _buffer[index];
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(MemoizedBuffer));
            }
        }
    }
}
