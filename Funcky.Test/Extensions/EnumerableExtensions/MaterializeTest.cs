using System.Collections;
using System.Collections.Immutable;
using Funcky.Test.TestUtilities;
using Xunit.Sdk;

namespace Funcky.Test.Extensions.EnumerableExtensions;

public sealed class MaterializeTest
{
    [Fact]
    public void MaterializeEnumeratesNonCollection()
    {
        var doNotEnumerate = new FailOnEnumerationSequence<object>();

        Assert.Throws<XunitException>(() => doNotEnumerate.Materialize());
    }

    [Fact]
    public void MaterializeDoesNotEnumerateCollectionTypes()
    {
        var list = new List<string> { "something" };

        Assert.IsType<List<string>>(list.Materialize());
        Assert.IsType<List<string>>(list.Materialize(ToHashSet));
    }

    [Fact]
    public void MaterializeReturnsImmutableCollectionWhenEnumerated()
    {
        var sequence = Enumerable.Repeat("Hello world!", 3).PreventLinqOptimizations();

        Assert.IsType<ImmutableList<string>>(sequence.Materialize());
    }

    [Fact]
    public void MaterializeWithMaterializationReturnsCorrectCollectionWhenEnumerate()
    {
        var sequence = Enumerable.Repeat("Hello world!", 3).PreventLinqOptimizations();

        Assert.IsType<HashSet<string>>(sequence.Materialize(ToHashSet));
    }

#if NET8_0_OR_GREATER
    // This is an optimization added in .NET 8
    [Fact]
    public void MaterializeDoesNotEnumerableEnumerableReturnedByRepeat()
    {
        var sequence = Enumerable.Repeat("Hello world!", 3);
        var materialized = sequence.Materialize<string, IReadOnlyCollection<string>>(_ => throw FailException.ForFailure("Materialization should never be called"));
        Assert.Same(sequence, materialized);
    }
#endif

    [Fact]
    public void MaterializeDoesNotEnumerateCollectionWhichImplementsICollectionOnly()
    {
        var list = new FailOnEnumerateCollection<int>(Count: 10);
        _ = list.Materialize();
    }

    [Fact]
    public void MaterializedICollectionHasCorrectCount()
    {
        const int count = 10;
        var list = new FailOnEnumerateCollection<int>(Count: count);
        Assert.Equal(count, list.Materialize().Count);
    }

    [Fact]
    public void MaterializingAnICollectionReturnsAnICollection()
    {
        var collection = new FailOnEnumerateCollection<int>(Count: 0);
        Assert.IsAssignableFrom<ICollection<int>>(collection.Materialize());
    }

    [Fact]
    public void MaterializingAnIListReturnsAnIList()
    {
        var collection = new FailOnEnumerateList<int>(Count: 0);
        Assert.IsAssignableFrom<IList<int>>(collection.Materialize());
    }

    [Fact]
    public void MaterializingAnIListReturnsAnIReadOnlyList()
    {
        var collection = new FailOnEnumerateList<int>(Count: 0);
        Assert.IsAssignableFrom<IReadOnlyList<int>>(collection.Materialize());
    }

    [Fact]
    public void MaterializedICollectionIsAReadOnlyView()
    {
        var source = new List<int> { 1, 2, 3 };
        var materialized = (ICollection<int>)new CollectionOnly<int>(source).Materialize();

        Assert.True(materialized.IsReadOnly);
        Assert.Throws<NotSupportedException>(() => materialized.Add(4));
        Assert.Throws<NotSupportedException>(() => materialized.Remove(1));
        Assert.Throws<NotSupportedException>(materialized.Clear);
        Assert.Equal([1, 2, 3], source);
    }

    [Fact]
    public void MaterializedIListIsAReadOnlyView()
    {
        var source = new List<int> { 1, 2, 3 };
        var materialized = (IList<int>)new ListOnly<int>(source).Materialize();

        Assert.True(materialized.IsReadOnly);
        Assert.Throws<NotSupportedException>(() => materialized[0] = 42);
        Assert.Throws<NotSupportedException>(() => materialized.Insert(0, 42));
        Assert.Throws<NotSupportedException>(() => materialized.RemoveAt(0));
        Assert.Equal([1, 2, 3], source);
    }

    [Fact]
    public void MaterializedCollectionReflectsLaterChangesToTheSource()
    {
        var source = new List<int> { 1, 2, 3 };
        var materialized = new ListOnly<int>(source).Materialize();

        source.Add(4);

        Assert.Equal(4, materialized.Count);
        Assert.Equal([1, 2, 3, 4], materialized);
    }

    [Fact]
    public void MaterializedCollectionSupportsReadOperations()
    {
        var source = new List<int> { 1, 2, 3 };
        var materialized = (IList<int>)new ListOnly<int>(source).Materialize();

        Assert.Equal(2, materialized[1]);
        Assert.Equal(2, materialized.IndexOf(3));
        Assert.Contains(3, materialized);
        var target = new int[4];
        materialized.CopyTo(target, 1);
        Assert.Equal([0, 1, 2, 3], target);
    }

    private static HashSet<string> ToHashSet(IEnumerable<string> s)
        => s.ToHashSet();

    private class CollectionOnly<T>(ICollection<T> collection) : ICollection<T>
    {
        public int Count => collection.Count;

        public bool IsReadOnly => collection.IsReadOnly;

        public IEnumerator<T> GetEnumerator() => collection.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(T item) => collection.Add(item);

        public void Clear() => collection.Clear();

        public bool Contains(T item) => collection.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => collection.CopyTo(array, arrayIndex);

        public bool Remove(T item) => collection.Remove(item);
    }

    private sealed class ListOnly<T>(IList<T> list) : CollectionOnly<T>(list), IList<T>
    {
        public T this[int index]
        {
            get => list[index];
            set => list[index] = value;
        }

        public int IndexOf(T item) => list.IndexOf(item);

        public void Insert(int index, T item) => list.Insert(index, item);

        public void RemoveAt(int index) => list.RemoveAt(index);
    }
}
