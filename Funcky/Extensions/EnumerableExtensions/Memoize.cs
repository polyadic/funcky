using System.Collections;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// Creates a buffer with a view over the source sequence, causing each enumerator to obtain access to all of the
        /// sequence's elements without causing multiple enumerations over the source.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements in <paramref name="sequence"/> sequence.</typeparam>
        /// <param name="sequence">The source sequence.</param>
        /// <returns>A lazy buffer of the underlying sequence.</returns>
        [Pure]
        public static IBuffer<TSource> Memoize<TSource>(this IEnumerable<TSource> sequence)
            where TSource : notnull
            => sequence is IBuffer<TSource> buffer
                ? buffer
                : MemoizedBuffer.Create(sequence);

        private static class MemoizedBuffer
        {
            public static MemoizedBuffer<TSource> Create<TSource>(IEnumerable<TSource> source)
                => new(source);
        }

        private sealed class MemoizedBuffer<T> : IBuffer<T>
        {
            private readonly List<T> _buffer = new();
            private readonly IEnumerator<T> _source;

            private bool _disposed;

            public MemoizedBuffer(IEnumerable<T> source)
                => _source = source.GetEnumerator();

            public IEnumerator<T> GetEnumerator()
                => _disposed
                ? throw new ObjectDisposedException("Buffer already disposed.")
                : GetEnumeratorInternal();

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
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

            private IEnumerator<T> GetEnumeratorInternal()
            {
                var index = 0;

                while (true)
                {
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
            }
        }
    }
}
