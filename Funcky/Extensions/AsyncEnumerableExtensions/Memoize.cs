#if INTEGRATED_ASYNC
#pragma warning disable SA1010 // StyleCop support for collection expressions is missing
namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <summary>
    /// Creates a buffer with a view over the source sequence, causing each enumerator to obtain access to all of the
    /// sequence's elements without causing multiple enumerations over the source.
    /// </summary>
    /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <returns>A lazy buffer of the underlying sequence.</returns>
    [Pure]
    public static IAsyncBuffer<TSource> Memoize<TSource>(this IAsyncEnumerable<TSource> source)
        => source is IAsyncBuffer<TSource> buffer
            ? Borrow(buffer)
            : MemoizedAsyncBuffer.Create(source);

    private static IAsyncBuffer<TSource> Borrow<TSource>(IAsyncBuffer<TSource> buffer)
        => new BorrowedAsyncBuffer<TSource>(buffer);

    private static class MemoizedAsyncBuffer
    {
        public static MemoizedAsyncBuffer<TSource> Create<TSource>(IAsyncEnumerable<TSource> source)
            => new(source);
    }

    private sealed class BorrowedAsyncBuffer<T>(IAsyncBuffer<T> inner) : IAsyncBuffer<T>
    {
        private bool _disposed;

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            return inner.GetAsyncEnumerator(cancellationToken);
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

    private sealed class MemoizedAsyncBuffer<T>(IAsyncEnumerable<T> source) : IAsyncBuffer<T>
    {
        private readonly List<T> _buffer = [];
        private readonly IAsyncEnumerator<T> _source = source.GetAsyncEnumerator();

        private bool _disposed;

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
#endif
