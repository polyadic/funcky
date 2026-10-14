using System.Collections;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    /// <summary>
    /// Materializes all the items of a lazy <see cref="IEnumerable{T}" />. If the underlying sequence is a collection type we do not actively enumerate them.
    /// </summary>
    /// <remarks>
    /// An existing collection is returned or wrapped, never copied, so the result is a read-only view that reflects
    /// later changes to the source. <c>Materialize</c> guarantees that a lazy sequence is evaluated only once, not that
    /// the caller holds a snapshot.
    /// </remarks>
    /// <typeparam name="TSource">Type of the items in the source sequence.</typeparam>
    /// <param name="source">The source sequence can be any <see cref="IEnumerable{T}" />.</param>
    /// <returns>A collection of the enumerated items.</returns>
    public static IReadOnlyCollection<TSource> Materialize<TSource>(this IEnumerable<TSource> source)
        => source.Materialize(DefaultMaterializer);

    /// <summary>
    /// Materializes all the items of a lazy <see cref="IEnumerable{T}" />. If the underlying sequence is a collection type we do not actively enumerate them.
    /// Via the materialize function you can chose how the enumeration is done when it is needed.
    /// </summary>
    /// <remarks>
    /// An existing collection is returned or wrapped, never copied, so the result is a read-only view that reflects
    /// later changes to the source. <c>Materialize</c> guarantees that a lazy sequence is evaluated only once, not that
    /// the caller holds a snapshot. The <paramref name="materializer"/> only runs for a genuinely lazy source.
    /// </remarks>
    /// <typeparam name="TSource">Type of the items in the source sequence.</typeparam>
    /// <typeparam name="TMaterialization">The type of the materialization target.</typeparam>
    /// <param name="source">The source sequence can be any <see cref="IEnumerable{T}" />.</param>
    /// <param name="materializer">A function which materializes a given sequence into a collection.</param>
    /// <returns>A collection of the enumerated items.</returns>
    public static IReadOnlyCollection<TSource> Materialize<TSource, TMaterialization>(
        this IEnumerable<TSource> source,
        Func<IEnumerable<TSource>, TMaterialization> materializer)
        where TMaterialization : IReadOnlyCollection<TSource>
        => source switch
        {
            IReadOnlyCollection<TSource> readOnlyCollection => readOnlyCollection,
            IList<TSource> list => new ListAsReadOnlyCollectionProxy<TSource>(list),
            ICollection<TSource> collection => new CollectionAsReadOnlyCollectionProxy<TSource>(collection),
            _ => materializer(source),
        };

    private static IReadOnlyCollection<TSource> DefaultMaterializer<TSource>(IEnumerable<TSource> source)
        => source.ToImmutableList();

    /// <summary>A read-only view over an <see cref="ICollection{T}"/>. It implements <see cref="ICollection{T}"/> too, so LINQ keeps its
    /// <c>Count</c> and <c>CopyTo</c> fast paths, but every mutating member throws like <see cref="ReadOnlyCollection{T}"/> does.</summary>
    private class CollectionAsReadOnlyCollectionProxy<T>(ICollection<T> collection) : ICollection<T>, IReadOnlyCollection<T>
    {
        public int Count => collection.Count;

        public bool IsReadOnly => true;

        public IEnumerator<T> GetEnumerator() => collection.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(T item) => throw new NotSupportedException();

        public void Clear() => throw new NotSupportedException();

        public bool Contains(T item) => collection.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => collection.CopyTo(array, arrayIndex);

        public bool Remove(T item) => throw new NotSupportedException();
    }

    private sealed class ListAsReadOnlyCollectionProxy<T>(IList<T> list)
        : CollectionAsReadOnlyCollectionProxy<T>(list), IList<T>, IReadOnlyList<T>
    {
        public T this[int index]
        {
            get => list[index];
            set => throw new NotSupportedException();
        }

        public int IndexOf(T item) => list.IndexOf(item);

        public void Insert(int index, T item) => throw new NotSupportedException();

        public void RemoveAt(int index) => throw new NotSupportedException();
    }
}
