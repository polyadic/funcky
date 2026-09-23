using System.Collections.Immutable;
using Funcky.Test.TestUtilities;

namespace Funcky.Test.Extensions.EnumerableExtensions;

public sealed class WithPreviousTest
{
    [Fact]
    public void WithPreviousIsEnumeratedLazily()
    {
        var doNotEnumerate = new FailOnEnumerationSequence<object>();

        _ = doNotEnumerate.WithPrevious();
    }

    [Fact]
    public void AnEmptySequenceWithPreviousReturnsAnEmptySequence()
    {
        var emptySequence = Enumerable.Empty<string>();
        Assert.Empty(emptySequence.WithPrevious());
    }

    [Fact]
    public void ASequenceWithOneElementWithPreviousHasOneElementWithNoPreviousElement()
    {
        const string expectedValue = "Hello world!";
        var oneElementSequence = Sequence.Return(expectedValue);
        var sequenceWithPrevious = oneElementSequence.WithPrevious();
        Assert.Collection(sequenceWithPrevious, value =>
        {
            Assert.Equal(expectedValue, value.Value);
            FunctionalAssert.None(value.Previous);
        });
    }

    [Fact]
    public void ASequenceWithMoreThanOneElementWithPreviousHasPreviousSetExceptOnFirstElement()
    {
        var sequence = ImmutableArray.Create("foo", "bar", "baz", "qux");
        var expectedSequenceWithPrevious = ImmutableArray.Create(
            new ValueWithPrevious<string>("foo", Option<string>.None),
            new ValueWithPrevious<string>("bar", "foo"),
            new ValueWithPrevious<string>("baz", "bar"),
            new ValueWithPrevious<string>("qux", "baz"));
        Assert.Equal(expectedSequenceWithPrevious, sequence.WithPrevious());
    }

    [Fact]
    public void ElementAtAccessIsOptimizedOnAnIListSourceWithIndex()
    {
        const int length = 200;
        var nonEnumerableList = new FailOnEnumerationList(length);
        var listWithLast = nonEnumerableList.WithPrevious();

        Assert.Equal(137, listWithLast.ElementAt(137).Value);

        Assert.Equal(0, listWithLast.ElementAt(0).Value);
        FunctionalAssert.None(listWithLast.ElementAt(0).Previous);

        foreach (var index in Enumerable.Range(1, length - 1))
        {
            CheckValues(listWithLast, index);
        }
    }

    [Fact]
    public void OptimizedSourceWithIndexCanBeEnumerated()
    {
        const int length = 222;
        var nonEnumerableList = Enumerable.Range(0, length).ToList();

        Assert.Equal(length, nonEnumerableList.WithPrevious().Aggregate(0, (sum, _) => sum + 1));
    }

    [Fact]
    public void CopyToOnAnOptimizedSourceWithPreviousWritesAllElementsStartingAtTheArrayIndex()
    {
        var collection = (ICollection<ValueWithPrevious<int>>)new List<int> { 10, 20, 30 }.WithPrevious();
        var array = new ValueWithPrevious<int>[5];

        collection.CopyTo(array, 2);

        Assert.Equal([(0, Option<int>.None), (0, Option<int>.None), (10, Option<int>.None), (20, Option.Some(10)), (30, Option.Some(20))], array.Select(v => (v.Value, v.Previous)));
    }

    private static void CheckValues(IEnumerable<ValueWithPrevious<int>> listWithLast, int index)
    {
        Assert.Equal(index, listWithLast.ElementAt(index).Value);

        var previous = FunctionalAssert.Some(listWithLast.ElementAt(index).Previous);
        Assert.Equal(index - 1, previous);
    }
}
