using System.Collections;

namespace Funcky;

public static partial class Sequence
{
    /// <summary>
    /// Generates a sequence that contains the same sequence of elements over and over again as an endless generator.
    /// </summary>
    /// <typeparam name="TItem">Type of the elements to be cycled.</typeparam>
    /// <param name="sequence">The sequence of elements which are cycled. Throws an exception if the sequence is empty.</param>
    /// <returns>Returns an infinite IEnumerable repeating the same sequence of elements.</returns>
    [Pure]
    public static IBuffer<TItem> CycleRange<TItem>(IEnumerable<TItem> sequence)
        where TItem : notnull
        => CycleBuffer.Create(sequence);

    private static class CycleBuffer
    {
        public static CycleBuffer<TSource> Create<TSource>(IEnumerable<TSource> source, Option<int> maxCycles = default)
            => new(source, maxCycles);
    }

    private sealed class CycleBuffer<T> : IBuffer<T>
    {
        private readonly List<T> _buffer = new();
        private readonly IEnumerator<T> _source;
        private readonly Option<int> _maxCycles;

        private bool _disposed;

        public CycleBuffer(IEnumerable<T> source, Option<int> maxCycles = default)
        {
            _source = source.GetEnumerator();
            _maxCycles = maxCycles;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _source.Dispose();
                _buffer.Clear();
            }

            _disposed = true;
        }

        public IEnumerator<T> GetEnumerator()
            => _disposed
            ? throw new ObjectDisposedException("Buffer already disposed.")
            : GetEnumeratorInternal();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private IEnumerator<T> GetEnumeratorInternal()
        {
            if (_maxCycles.Match(none: false, some: maxCycles => maxCycles is 0))
            {
                yield break;
            }

            var index = 0;

            while (true)
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException("Buffer already disposed.");
                }

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

                yield return _buffer[index++];
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

            for (int cycle = 1; IsCycling(cycle); ++cycle)
            {
                foreach (var value in _buffer)
                {
                    yield return value;
                }
            }
        }

        private bool IsCycling(int cycle)
            => _maxCycles.Match(
                none: true,
                some: maxCycles => cycle < maxCycles);
    }
}
