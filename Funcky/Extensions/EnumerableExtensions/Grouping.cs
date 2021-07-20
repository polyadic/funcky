using System.Collections;
using System.Collections.Immutable;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        internal class Grouping<TKey, TElement> : IGrouping<TKey, TElement>, IReadOnlyList<TElement>, IList<TElement>
        {
            private readonly IImmutableList<TElement> _elements;

            internal Grouping(TKey key, IImmutableList<TElement> elements)
            {
                Key = key;
                _elements = elements;
            }

            public TKey Key { get; }

            public int Count => _elements.Count;

            public bool IsReadOnly => true;

            public TElement this[int index]
            {
                get => _elements[index];
                set => throw new NotSupportedException();
            }

            public void Add(TElement item)
                => throw new NotSupportedException();

            public void Clear()
                => throw new NotSupportedException();

            public bool Contains(TElement element)
                => _elements.Contains(element);

            public void CopyTo(TElement[] array, int arrayIndex)
                => throw new NotSupportedException();

            public IEnumerator<TElement> GetEnumerator()
                => _elements.GetEnumerator();

            public int IndexOf(TElement element)
                => _elements.IndexOf(element);

            public void Insert(int index, TElement element)
                => throw new NotSupportedException();

            public bool Remove(TElement element)
                => throw new NotSupportedException();

            public void RemoveAt(int index)
                => throw new NotSupportedException();

            IEnumerator IEnumerable.GetEnumerator()
                => GetEnumerator();
        }
    }
}
