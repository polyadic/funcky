namespace Funcky;

public static partial class AsyncSequence
{
    /// <summary>
    /// Generates a sequence that contains the same sequence of elements over and over again as an endless generator.
    /// </summary>
    /// <typeparam name="TItem">Type of the elements to be cycled.</typeparam>
    /// <param name="sequence">The sequence of elements which are cycled. Throws an exception if the sequence is empty.</param>
    /// <returns>Returns an infinite IEnumerable repeating the same sequence of elements.</returns>
    [Pure]
    public static IAsyncBuffer<TItem> CycleRange<TItem>(IAsyncEnumerable<TItem> sequence)
        where TItem : notnull
        => CycleBuffer.Create(sequence);

    private static class CycleBuffer
    {
        public static AsyncCycleBuffer<TSource> Create<TSource>(IAsyncEnumerable<TSource> source, Option<int> maxCycles = default)
            => new(source, maxCycles);
    }

    private sealed class AsyncCycleBuffer<T> : IAsyncBuffer<T>
    {
        private readonly List<T> _buffer = new();
        private readonly IAsyncEnumerator<T> _source;
        private readonly Option<int> _maxCycles;

        private bool _disposed;

        public AsyncCycleBuffer(IAsyncEnumerable<T> source, Option<int> maxCycles = default)
            => (_source, _maxCycles) = (source.GetAsyncEnumerator(), maxCycles);

        public async ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                await _source.DisposeAsync().ConfigureAwait(false);
                _buffer.Clear();
                _disposed = true;
            }
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            return GetEnumeratorInternal();
        }

        private async IAsyncEnumerator<T> GetEnumeratorInternal()
        {
            if (HasNoCycles())
            {
                yield break;
            }

            for (var index = 0; true; ++index)
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

            if (_buffer.Count is 0)
            {
                if (_maxCycles.Match(none: true, some: False))
                {
                    throw new InvalidOperationException("you cannot cycle an empty enumerable");
                }
                else
                {
                    yield break;
                }
            }

            // this can change on Dispose!
            var bufferCount = _buffer.Count;

            for (int cycle = 1; IsCycling(cycle); ++cycle)
            {
                for (var index = 0; index < bufferCount; ++index)
                {
                    ThrowIfDisposed();

                    yield return _buffer[index];
                }
            }
        }

        private bool HasNoCycles()
            => _maxCycles.Match(none: false, some: maxCycles => maxCycles is 0);

        private bool IsCycling(int cycle)
            => _maxCycles.Match(
                none: true,
                some: maxCycles => cycle < maxCycles);

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(CycleBuffer));
            }
        }
    }
}
