using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        [Pure]
        public static IEnumerable<IEnumerable<TSource>> AdjacentGroupBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
            => AdjacentGroupBy(source, keySelector, EqualityComparer<TKey>.Default);

        [Pure]
        public static IEnumerable<IGrouping<TKey, TSource>> AdjacentGroupBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            using var enumerator = source.GetEnumerator();

            if (!enumerator.MoveNext())
            {
                yield break;
            }

            var group = CreateGrouping(keySelector(enumerator.Current), enumerator.Current);

            while (enumerator.MoveNext())
            {
                if (comparer.Equals(group.Key, keySelector(enumerator.Current)))
                {
                    group.Add(enumerator.Current);
                }
                else
                {
                    yield return group;
                    group = CreateGrouping(keySelector(enumerator.Current), enumerator.Current);
                }
            }

            yield return group;
        }

        internal static Grouping<TKey, TElement> CreateGrouping<TKey, TElement>(TKey key, TElement element)
        {
            return new Grouping<TKey, TElement>(key, element);
        }

        public class Grouping<TKey, TElement> : IGrouping<TKey, TElement>, IList<TElement>
        {
            private readonly List<TElement> _elements = new List<TElement>();

            internal Grouping(TKey key, TElement element)
            {
                Key = key;
                _elements.Add(element);
            }

            public TKey Key { get; }

            public int Count
                => _elements.Count;

            public bool IsReadOnly
                => true;

            public TElement this[int index]
            {
                get => _elements[index];
                set => throw new NotImplementedException();
            }

            public void Add(TElement element)
                => _elements.Add(element);

            public void Clear()
                => throw new NotSupportedException();

            public bool Contains(TElement element)
                => _elements.Contains(element);

            public void CopyTo(TElement[] array, int arrayIndex)
                => throw new NotImplementedException();

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
