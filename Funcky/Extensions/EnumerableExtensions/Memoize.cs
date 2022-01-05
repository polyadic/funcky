#pragma warning disable IDISP005 // Return type should indicate that the value should be disposed.
#pragma warning disable IDISP006 // Implement IDisposable.
#pragma warning disable IDISP009 // Add IDisposable interface.

using System.Collections;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        [Pure]
        public static IReadOnlyList<T> Memoize<T>(this IEnumerable<T> sequence)
            => sequence switch
            {
                IReadOnlyList<T> list => list,
                _ => new BufferedSequence<T>(sequence),
            };

        internal sealed class BufferedSequence<T> : IReadOnlyList<T>
        {
            private readonly IEnumerator<T> _enumerator;
            private readonly List<T> _cache = new();
            private bool _disposed;

            public BufferedSequence(IEnumerable<T> sequence)
                => _enumerator = sequence.GetEnumerator();

            public int Count => _disposed
                ? _cache.Count
                : throw new NotSupportedException("not enumerated completely");

            public T this[int index] => index >= 0 && index < _cache.Count
                ? _cache[index]
                : throw new NotSupportedException("not enumerated (yet)");

            public IEnumerator<T> GetEnumerator()
            {
                foreach (var element in _cache)
                {
                    yield return element;
                }

                while (_enumerator.MoveNext())
                {
                    var current = _enumerator.Current;
                    _cache.Add(current);
                    yield return current;
                }

                _enumerator.Dispose();
                _disposed = true;
            }

            IEnumerator IEnumerable.GetEnumerator()
                => GetEnumerator();

            public void Dispose()
                => _enumerator.Dispose();
        }
    }
}
