using System.Diagnostics.CodeAnalysis;
using Funcky.Test.TestUtils;

namespace Funcky.Test.Extensions.EnumerableExtensions;

public sealed class WithFirstTest
{
    [Fact]
    public void WithFirstIsEnumeratedLazily()
    {
        var doNotEnumerate = new FailOnEnumerationSequence<object>();

        _ = doNotEnumerate.WithFirst();
    }

    [Fact]
    public void AnEmptySequenceWithFirstReturnsAnEmptySequence()
    {
        var emptySequence = Enumerable.Empty<string>();

        Assert.Empty(emptySequence.WithFirst());
    }

    [Fact]
    public void ASequenceWithOneElementWithFirstHasOneElementWhichIsMarkedFirst()
    {
        const string expectedValue = "Hello world!";
        var oneElementSequence = Sequence.Return(expectedValue);

        var sequenceWithFirst = oneElementSequence.WithFirst();
        foreach (var (value, isFirst) in sequenceWithFirst)
        {
            Assert.Equal(expectedValue, value);
            Assert.True(isFirst);
        }

        Assert.NotEmpty(sequenceWithFirst);
    }

    [Fact]
    public void ASequenceWithMultipleElementsWithFirstMarksTheFirstElement()
    {
        const int length = 20;

        var sequence = Enumerable.Range(1, length);

        foreach (var valueWithFirst in sequence.WithFirst())
        {
            Assert.Equal(valueWithFirst.Value == 1, valueWithFirst.IsFirst);
        }
    }

    [Fact]
    public void ElementAtAccessIsOptimizedOnAnIListSourceWithIndex()
    {
        var length = 5000;
        var nonEnumerableList = new FailOnEnumerationList(length);
        var listWithLast = nonEnumerableList.WithFirst();

        Assert.Equal(1337, listWithLast.ElementAt(1337).Value);

        Assert.True(listWithLast.ElementAt(0).IsFirst);
        Assert.False(listWithLast.ElementAt(1).IsFirst);
        Assert.False(listWithLast.ElementAt(2500).IsFirst);
        Assert.False(listWithLast.ElementAt(length - 2).IsFirst);
        Assert.False(listWithLast.ElementAt(length - 1).IsFirst);
    }

    [Fact]
    public void OptimizedSourceWithIndexCanBeEnumerated()
    {
        var length = 222;
        var nonEnumerableList = Enumerable.Range(0, length).ToList();

        Assert.Equal(length, nonEnumerableList.WithFirst().Aggregate(0, (sum, _) => sum + 1));
    }

    [Fact]
    [SuppressMessage("Assertions", "xUnit2017:Do not use Contains() to check if a value exists in a collection")]
    public void ContainsWorksOnListWithFirst()
    {
        var sequence = Sequence.Return(1, 2, 3).ToList();
        Assert.True(sequence.WithFirst().Contains(new ValueWithFirst<int>(1, isFirst: true)));
    }
}
