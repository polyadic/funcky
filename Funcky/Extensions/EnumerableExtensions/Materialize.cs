using System.Collections;
using System.Collections.Immutable;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// Materializes all the items of a lazy <see cref="IEnumerable{TItem}" />. If the underlying sequence is a collection type we do not actively enumerate them.
        /// </summary>
        /// <typeparam name="TItem">Type of the items in the source sequence.</typeparam>
        /// <param name="source">The source sequence can be any <see cref="IEnumerable{TItem}" />.</param>
        /// <returns>A collection of the enumerated items.</returns>
        public static IReadOnlyCollection<TItem> Materialize<TItem>(this IEnumerable<TItem> source)
            => source.Materialize(DefaultMaterialization);

        /// <summary>
        /// Materializes all the items of a lazy <see cref="IEnumerable{TItem}" />. If the underlying sequence is a collection type we do not actively enumerate them.
        /// Via the materialize function you can chose how the enumeration is done when it is needed.
        /// </summary>
        /// <typeparam name="TItem">Type of the items in the source sequence.</typeparam>
        /// <typeparam name="TMaterialization">The type of the materialization target.</typeparam>
        /// <param name="source">The source sequence can be any <see cref="IEnumerable{TItem}" />.</param>
        /// <param name="materialize">A function which materializes a given sequence into a collection.</param>
        /// <returns>A collection of the enumerated items.</returns>
        public static IReadOnlyCollection<TItem> Materialize<TItem, TMaterialization>(
            this IEnumerable<TItem> source,
            Func<IEnumerable<TItem>, TMaterialization> materialize)
            where TMaterialization : IReadOnlyCollection<TItem>
            => source switch
            {
                IReadOnlyCollection<TItem> readOnlyCollection => readOnlyCollection,
                IList<TItem> list => new ListAsReadOnlyCollectionProxy<TItem>(list),
                ICollection<TItem> collection => new CollectionAsReadOnlyCollectionProxy<TItem>(collection),
                _ => materialize(source),
            };

        private static IReadOnlyCollection<TItem> DefaultMaterialization<TItem>(IEnumerable<TItem> source)
            => source.ToImmutableList();

        private class CollectionAsReadOnlyCollectionProxy<T> : ICollection<T>, IReadOnlyCollection<T>
        {
            private readonly ICollection<T> _collection;

            public CollectionAsReadOnlyCollectionProxy(ICollection<T> collection) => _collection = collection;

            public int Count => _collection.Count;

            public bool IsReadOnly => _collection.IsReadOnly;

            public IEnumerator<T> GetEnumerator() => _collection.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public void Add(T item) => _collection.Add(item);

            public void Clear() => _collection.Clear();

            public bool Contains(T item) => _collection.Contains(item);

            public void CopyTo(T[] array, int arrayIndex) => _collection.CopyTo(array, arrayIndex);

            public bool Remove(T item) => _collection.Remove(item);
        }

        private sealed class ListAsReadOnlyCollectionProxy<T> : CollectionAsReadOnlyCollectionProxy<T>, IList<T>, IReadOnlyList<T>
        {
            private readonly IList<T> _list;

            public ListAsReadOnlyCollectionProxy(IList<T> list)
                : base(list) => _list = list;

            public T this[int index]
            {
                get => _list[index];
                set => _list[index] = value;
            }

            public int IndexOf(T item) => _list.IndexOf(item);

            public void Insert(int index, T item) => _list.Insert(index, item);

            public void RemoveAt(int index) => _list.RemoveAt(index);
        }
    }
}
