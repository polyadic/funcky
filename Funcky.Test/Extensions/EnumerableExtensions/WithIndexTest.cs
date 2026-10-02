using Funcky.Test.TestUtilities;

namespace Funcky.Test.Extensions.EnumerableExtensions;

public sealed class WithIndexTest
{
    [Fact]
    public void WithIndexIsEnumeratedLazily()
    {
        var doNotEnumerate = new FailOnEnumerationSequence<object>();

        _ = doNotEnumerate.WithIndex();
    }

    [Fact]
    public void AnEmptySequenceWithIndexReturnsAnEmptySequence()
    {
        var emptySequence = Enumerable.Empty<string>();

        Assert.Empty(emptySequence.WithIndex());
    }

    [Fact]
    public void ASequenceWithOneElementWithIndexHasTheIndexZero()
    {
        const string expectedValue = "Hello world!";
        var oneElementSequence = Sequence.Return(expectedValue);

        var sequenceWithIndex = oneElementSequence.WithIndex();
        foreach (var (value, index) in sequenceWithIndex)
        {
            Assert.Equal(expectedValue, value);
            Assert.Equal(0, index);
        }

        Assert.NotEmpty(sequenceWithIndex);
    }

    [Fact]
    public void ASequenceWithMultipleElementsWithIndexHaveAscendingIndices()
    {
        var sequence = Enumerable.Range(0, 20);

        foreach (var valueWithIndex in sequence.WithIndex())
        {
            Assert.Equal(valueWithIndex.Value, valueWithIndex.Index);
        }
    }

    [Fact]
    public void ElementAtAccessIsOptimizedOnAnIListSourceWithIndex()
    {
        var nonEnumerableList = new FailOnEnumerationList(5000);
        var listWithIndex = nonEnumerableList.WithIndex();

        Assert.Equal(1337, listWithIndex.ElementAt(1337).Value);
        Assert.Equal(listWithIndex.ElementAt(999).Value, listWithIndex.ElementAt(999).Index);
    }

    [Fact]
    public void OptimizedSourceWithIndexCanBeEnumerated()
    {
        const int length = 222;
        var nonEnumerableList = Enumerable.Range(0, length).ToList();

        Assert.Equal(length, nonEnumerableList.WithIndex().Aggregate(0, (sum, _) => sum + 1));
    }

    [Fact]
    public void AddingAnOptimizedSourceWithIndexToANonEmptyListCopiesAllElements()
    {
        var target = new List<ValueWithIndex<int>> { new(1, 0), new(2, 1) };

        target.AddRange(new List<int> { 10, 20, 30 }.WithIndex());

        Assert.Equal([(1, 0), (2, 1), (10, 0), (20, 1), (30, 2)], target.Select(v => (v.Value, v.Index)));
    }

    [Fact]
    public void CopyToOnAnOptimizedSourceWithIndexWritesAllElementsStartingAtTheArrayIndex()
    {
        var collection = (ICollection<ValueWithIndex<int>>)new List<int> { 10, 20, 30 }.WithIndex();
        var array = new ValueWithIndex<int>[5];

        collection.CopyTo(array, 2);

        Assert.Equal([(0, 0), (0, 0), (10, 0), (20, 1), (30, 2)], array.Select(v => (v.Value, v.Index)));
    }
}
