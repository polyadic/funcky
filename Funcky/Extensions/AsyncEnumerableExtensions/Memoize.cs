#if ASYNC_SUPPORTED
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
    /// <remarks>
    /// The source is not enumerated before the first element is requested. The buffer can be enumerated by several
    /// consumers concurrently; the source is still pulled one element at a time and each element is produced exactly once.
    /// The cancellation token passed to <see cref="IAsyncEnumerable{T}.GetAsyncEnumerator"/> cancels that consumer's
    /// enumeration, it does not affect the buffer or other consumers.
    /// </remarks>
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
        private readonly SemaphoreSlim _sourceLock = new(initialCount: 1, maxCount: 1);

        private IAsyncEnumerator<T>? _source;
        private bool _sourceExhausted;
        private bool _disposed;

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            return GetAsyncEnumeratorInternal(cancellationToken);
        }

        public async ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                _disposed = true;

                if (_source is { } source)
                {
                    await source.DisposeAsync().ConfigureAwait(false);
                }

                _buffer.Clear();
                _sourceLock.Dispose();
            }
        }

        private async IAsyncEnumerator<T> GetAsyncEnumeratorInternal(CancellationToken cancellationToken)
        {
            for (var index = 0; true; index++)
            {
                var (hasElement, element) = await GetElementAsync(index, cancellationToken).ConfigureAwait(false);

                if (!hasElement)
                {
                    yield break;
                }

                yield return element!;
            }
        }

        private async ValueTask<(bool HasElement, T? Element)> GetElementAsync(int index, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            await _sourceLock.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                ThrowIfDisposed();

                return index < _buffer.Count
                    ? (true, _buffer[index])
                    : await FetchNextAsync().ConfigureAwait(false);
            }
            finally
            {
                _sourceLock.Release();
            }
        }

        // Must only be called while holding the source lock.
        private async ValueTask<(bool HasElement, T? Element)> FetchNextAsync()
        {
            if (_sourceExhausted)
            {
                return (false, default);
            }

            _source ??= source.GetAsyncEnumerator(CancellationToken.None);

            if (await _source.MoveNextAsync().ConfigureAwait(false))
            {
                _buffer.Add(_source.Current);
                return (true, _source.Current);
            }

            _sourceExhausted = true;
            return (false, default);
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
