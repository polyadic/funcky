#pragma warning disable SA1010 // StyleCop support for collection expressions is missing
using System.Collections;

namespace Funcky;

public static partial class Sequence
{
    /// <summary>
    /// Generates a sequence that contains the same sequence of elements over and over again as an endless generator.
    /// </summary>
    /// <typeparam name="TSource">Type of the elements to be cycled.</typeparam>
    /// <param name="source">The sequence of elements which are cycled. Throws an exception if the sequence is empty.</param>
    /// <returns>Returns an infinite IEnumerable repeating the same sequence of elements.</returns>
    /// <remarks>
    /// Use <see cref="CycleMaterialized{TSource}"/> if you need to cycle an already materialized sequence.
    /// The source is not enumerated before the first element is requested, and it is enumerated only once.
    /// </remarks>
    [Pure]
    public static IBuffer<TSource> CycleRange<TSource>(IEnumerable<TSource> source)
        => CycleBuffer.Create(source);

    private static class CycleBuffer
    {
        public static CycleBuffer<TSource> Create<TSource>(IEnumerable<TSource> source, Option<int> maxCycles = default)
            => new(source, maxCycles);
    }

    private sealed class CycleBuffer<T>(IEnumerable<T> source, Option<int> maxCycles = default) : IBuffer<T>
    {
        private readonly List<T> _buffer = [];

        private IEnumerator<T>? _source;
        private bool _sourceExhausted;
        private bool _disposed;

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                DisposeSource();
                _buffer.Clear();
            }
        }

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

        private IEnumerator<T> GetEnumeratorInternal()
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
                    if (TryFetchNext())
                    {
                        _buffer.Add(_source!.Current);
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
                if (maxCycles.Match(none: true, some: False))
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

        private bool TryFetchNext()
        {
            if (_sourceExhausted)
            {
                return false;
            }

            _source ??= source.GetEnumerator();

            if (_source.MoveNext())
            {
                return true;
            }

            _sourceExhausted = true;
            DisposeSource();
            return false;
        }

        private void DisposeSource()
        {
            _source?.Dispose();
            _source = null;
        }

        private bool HasNoCycles()
            => maxCycles.Match(none: false, some: maxCycles => maxCycles is 0);

        private bool IsCycling(int cycle)
            => maxCycles.Match(
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
