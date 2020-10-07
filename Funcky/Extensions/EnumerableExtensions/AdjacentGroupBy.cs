using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq;
using static Funcky.Functional;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// Groups adjacent elements of a source sequence with same key specified by the key selector function.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <returns>An IEnumerable{IGrouping{TKey, TSource}}  where each IGrouping{TKey,TElement} object contains a sequence of objects and a key.</returns>
        [Pure]
        public static IEnumerable<IGrouping<TKey, TSource>> AdjacentGroupBy<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
            => AdjacentGroupBy(source, keySelector, Identity, CreateGrouping, EqualityComparer<TKey>.Default);

        /// <summary>
        /// Groups adjacent elements of a source sequence according to a specified key selector function and compares the keys by using a specified comparer.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <param name="comparer">An IEqualityComparer{T} to compare keys.</param>
        /// <returns>An IEnumerable{IGrouping{TKey, TSource}}  where each IGrouping{TKey,TElement} object contains a sequence of objects and a key.</returns>
        [Pure]
        public static IEnumerable<IGrouping<TKey, TSource>> AdjacentGroupBy<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            IEqualityComparer<TKey> comparer)
            => AdjacentGroupBy(source, keySelector, Identity, CreateGrouping, comparer);

        /// <summary>
        /// Groups adjacent elements of a source sequence according to a key selector function. The keys are compared by using a comparer and each group's elements are projected by using a specified function.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        /// <typeparam name="TElement">The type of the elements in each IGrouping{TKey,TElement}.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <param name="elementSelector">A function to map each source element to an element in the IGrouping{TKey,TElement}.</param>
        /// <returns>An IEnumerable{IGrouping{TKey, TSource}}  where each IGrouping{TKey,TElement} object contains a sequence of objects and a key.</returns>
        [Pure]
        public static IEnumerable<IGrouping<TKey, TElement>> AdjacentGroupBy<TSource, TKey, TElement>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector)
            => AdjacentGroupBy(source, keySelector, elementSelector, CreateGrouping, EqualityComparer<TKey>.Default);

        /// <summary>
        /// Groups adjacent elements of a source sequence according to a specified key selector function and projects the elements for each group by using a specified function.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        /// <typeparam name="TElement">The type of the elements in each IGrouping{TKey,TElement}.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <param name="elementSelector">A function to map each source element to an element in the IGrouping{TKey,TElement}.</param>
        /// <param name="comparer">An IEqualityComparer{T} to compare keys.</param>
        /// <returns>An IEnumerable{IGrouping{TKey, TSource}}  where each IGrouping{TKey,TElement} object contains a sequence of objects and a key.</returns>
        [Pure]
        public static IEnumerable<IGrouping<TKey, TElement>> AdjacentGroupBy<TSource, TKey, TElement>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            IEqualityComparer<TKey> comparer)
            => AdjacentGroupBy(source, keySelector, elementSelector, CreateGrouping, comparer);

        /// <summary>
        /// Groups adjacent elements of a source sequence according to a specified key selector function and creates a result value from each group and its key.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        /// <typeparam name="TResult">The type of the result value returned by resultSelector.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <param name="resultSelector">A function to map each source element to an element in the IGrouping{TKey,TElement}.</param>
        /// <returns>A collection of elements of type TResult where each element represents a projection over a group and its key.</returns>
        [Pure]
        public static IEnumerable<TResult> AdjacentGroupBy<TSource, TKey, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TKey, IEnumerable<TSource>, TResult> resultSelector)
            => AdjacentGroupBy(source, keySelector, Identity, resultSelector, EqualityComparer<TKey>.Default);

        /// <summary>
        /// Groups adjacent elements of a source sequence according to a specified key selector function and creates a result value from each group and its key. The elements of each group are projected by using a specified function.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        /// <typeparam name="TElement">The type of the elements in each IGrouping{TKey,TElement}.</typeparam>
        /// <typeparam name="TResult">The type of the result value returned by resultSelector.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <param name="elementSelector">A function to map each source element to an element in the IGrouping{TKey,TElement}.</param>
        /// <param name="resultSelector">A function to create a result value from each group.</param>
        /// <returns>A collection of elements of type TResult where each element represents a projection over a group and its key.</returns>
        [Pure]
        public static IEnumerable<TResult> AdjacentGroupBy<TSource, TKey, TElement, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
            => AdjacentGroupBy(source, keySelector, elementSelector, resultSelector, EqualityComparer<TKey>.Default);

        /// <summary>
        /// Groups adjacent elements of a source sequence according to a specified key selector function and creates a result value from each group and its key. The keys are compared by using a specified comparer.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        /// <typeparam name="TResult">The type of the result value returned by resultSelector.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <param name="resultSelector">A function to create a result value from each group.</param>
        /// <param name="comparer">An IEqualityComparer{T} to compare keys.</param>
        /// <returns>A collection of elements of type TResult where each element represents a projection over a group and its key.</returns>
        [Pure]
        public static IEnumerable<TResult> AdjacentGroupBy<TSource, TKey, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TKey, IEnumerable<TSource>, TResult> resultSelector,
            IEqualityComparer<TKey> comparer)
            => AdjacentGroupBy(source, keySelector, Identity, resultSelector, comparer);

        /// <summary>
        /// Groups adjacent elements of a source sequence according to a specified key selector function and creates a result value from each group and its key. Key values are compared by using a specified comparer, and the elements of each group are projected by using a specified function.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        /// <typeparam name="TElement">The type of the elements in each IGrouping{TKey,TElement}.</typeparam>
        /// <typeparam name="TResult">The type of the result value returned by resultSelector.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <param name="elementSelector">A function to map each source element to an element in the IGrouping{TKey,TElement}.</param>
        /// <param name="resultSelector">A function to create a result value from each group.</param>
        /// <param name="comparer">An IEqualityComparer{T} to compare keys.</param>
        /// <returns>A collection of elements of type TResult where each element represents a projection over a group and its key.</returns>
        [Pure]
        public static IEnumerable<TResult> AdjacentGroupBy<TSource, TKey, TElement, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            Func<TKey, IReadOnlyList<TElement>, TResult> resultSelector,
            IEqualityComparer<TKey> comparer)
        {
            using var enumerator = source.GetEnumerator();

            if (!enumerator.MoveNext())
            {
                yield break;
            }

            var (group, key) = CreateGroupAndKey(keySelector, elementSelector, enumerator);

            while (enumerator.MoveNext())
            {
                if (comparer.Equals(key, keySelector(enumerator.Current)))
                {
                    group = group.Add(elementSelector(enumerator.Current));
                }
                else
                {
                    yield return resultSelector(key, group);
                    (group, key) = CreateGroupAndKey(keySelector, elementSelector, enumerator);
                }
            }

            yield return resultSelector(key, group);
        }

        internal static Grouping<TKey, TElement> CreateGrouping<TKey, TElement>(TKey key, IReadOnlyList<TElement> elements)
            => new Grouping<TKey, TElement>(key, elements);

        private static (ImmutableList<TElement> Group, TKey Key) CreateGroupAndKey<TSource, TKey, TElement>(Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEnumerator<TSource> enumerator)
        {
            var group = ImmutableList.Create(elementSelector(enumerator.Current));
            var key = keySelector(enumerator.Current);

            return (group, key);
        }

        internal class Grouping<TKey, TElement> : IGrouping<TKey, TElement>, IReadOnlyList<TElement>
        {
            private readonly IReadOnlyList<TElement> _elements;

            internal Grouping(TKey key, IReadOnlyList<TElement> elements)
            {
                Key = key;
                _elements = elements;
            }

            public TKey Key { get; }

            public int Count => _elements.Count;

            public TElement this[int index]
            {
                get => _elements[index];
                set => throw new NotSupportedException();
            }

            public IEnumerator<TElement> GetEnumerator()
                => _elements.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator()
                => GetEnumerator();
        }
    }
}
