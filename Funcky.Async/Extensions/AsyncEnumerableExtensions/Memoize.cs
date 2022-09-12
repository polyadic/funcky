namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <summary>
    /// Creates a buffer with a view over the source sequence, causing each enumerator to obtain access to all of the
    /// sequence's elements without causing multiple enumerations over the source.
    /// </summary>
    /// <typeparam name="TSource">Type of the elements in <paramref name="sequence"/> sequence.</typeparam>
    /// <param name="sequence">The source sequence.</param>
    /// <returns>A lazy buffer of the underlying sequence.</returns>
    [Pure]
    public static IAsyncBuffer<TSource> Memoize<TSource>(this IAsyncEnumerable<TSource> sequence)
        where TSource : notnull
        => sequence is IAsyncBuffer<TSource> buffer
            ? Borrow(buffer)
            : MemoizedAsyncBuffer.Create(sequence);

    private static IAsyncBuffer<TSource> Borrow<TSource>(IAsyncBuffer<TSource> buffer)
        => buffer as BorrowedAsyncBuffer<TSource> ?? new BorrowedAsyncBuffer<TSource>(buffer);

    private static class MemoizedAsyncBuffer
    {
        public static MemoizedAsyncBuffer<TSource> Create<TSource>(IAsyncEnumerable<TSource> source)
            => new(source);
    }

    private sealed class BorrowedAsyncBuffer<T> : IAsyncBuffer<T>
    {
        private readonly IAsyncBuffer<T> _inner;
        private bool _disposed;

        public BorrowedAsyncBuffer(IAsyncBuffer<T> inner) => _inner = inner;

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            return _inner.GetAsyncEnumerator(cancellationToken);
        }

        public ValueTask DisposeAsync()
        {
            _disposed = true;
            return default;
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(BorrowedAsyncBuffer<T>));
            }
        }
    }

    private sealed class MemoizedAsyncBuffer<T> : IAsyncBuffer<T>
    {
        private readonly List<T> _buffer = new();
        private readonly IAsyncEnumerator<T> _source;

        private bool _disposed;

        public MemoizedAsyncBuffer(IAsyncEnumerable<T> source)
            => _source = source.GetAsyncEnumerator();

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            return GetAsyncEnumeratorInternal();
        }

        public async ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                await _source.DisposeAsync().ConfigureAwait(false);
                _buffer.Clear();
                _disposed = true;
            }
        }

        private async IAsyncEnumerator<T> GetAsyncEnumeratorInternal()
        {
            for (var index = 0; true; index++)
            {
                ThrowIfDisposed();

                if (index == _buffer.Count)
                {
                    if (await _source.MoveNextAsync().ConfigureAwait(false))
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
                throw new ObjectDisposedException(nameof(MemoizedAsyncBuffer));
            }
        }
    }
}
