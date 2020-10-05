using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using static Funcky.Functional;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        [Pure]
        public static IEnumerable<IGrouping<TKey, TSource>> AdjacentGroupBy<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
            => AdjacentGroupBy(source, keySelector, Identity, CreateGrouping, EqualityComparer<TKey>.Default);

        [Pure]
        public static IEnumerable<IGrouping<TKey, TSource>> AdjacentGroupBy<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            IEqualityComparer<TKey> comparer)
            => AdjacentGroupBy(source, keySelector, Identity, CreateGrouping, comparer);

        [Pure]
        public static IEnumerable<IGrouping<TKey, TElement>> AdjacentGroupBy<TSource, TKey, TElement>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector)
            => AdjacentGroupBy(source, keySelector, elementSelector, CreateGrouping, EqualityComparer<TKey>.Default);

        [Pure]
        public static IEnumerable<IGrouping<TKey, TElement>> AdjacentGroupBy<TSource, TKey, TElement>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            IEqualityComparer<TKey> comparer)
            => AdjacentGroupBy(source, keySelector, elementSelector, CreateGrouping, comparer);

        [Pure]
        public static IEnumerable<TResult> AdjacentGroupBy<TSource, TKey, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TKey, IEnumerable<TSource>, TResult> resultSelector)
            => AdjacentGroupBy(source, keySelector, Identity, resultSelector, EqualityComparer<TKey>.Default);

        [Pure]
        public static IEnumerable<TResult> AdjacentGroupBy<TSource, TKey, TElement, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
            => AdjacentGroupBy(source, keySelector, elementSelector, resultSelector, EqualityComparer<TKey>.Default);

        [Pure]
        public static IEnumerable<TResult> AdjacentGroupBy<TSource, TKey, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TKey, IEnumerable<TSource>, TResult> resultSelector,
            IEqualityComparer<TKey> comparer)
            => AdjacentGroupBy(source, keySelector, Identity, resultSelector, comparer);

        [Pure]
        public static IEnumerable<TResult> AdjacentGroupBy<TSource, TKey, TElement, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            Func<TKey, IList<TElement>, TResult> resultSelector,
            IEqualityComparer<TKey> comparer)
        {
            using var enumerator = source.GetEnumerator();

            if (!enumerator.MoveNext())
            {
                yield break;
            }

            var group = new List<TElement>() { elementSelector(enumerator.Current) };
            var key = keySelector(enumerator.Current);

            while (enumerator.MoveNext())
            {
                if (comparer.Equals(key, keySelector(enumerator.Current)))
                {
                    group.Add(elementSelector(enumerator.Current));
                }
                else
                {
                    yield return resultSelector(key, group);
                    group = new List<TElement>() { elementSelector(enumerator.Current) };
                    key = keySelector(enumerator.Current);
                }
            }

            yield return resultSelector(key, group);
        }

        internal static Grouping<TKey, TElement> CreateGrouping<TKey, TElement>(TKey key, IList<TElement> elements)
        {
            return new Grouping<TKey, TElement>(key, elements);
        }

        public class Grouping<TKey, TElement> : IGrouping<TKey, TElement>, IList<TElement>
        {
            private readonly IList<TElement> _elements;

            internal Grouping(TKey key, IList<TElement> elements)
            {
                Key = key;
                _elements = elements;
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
