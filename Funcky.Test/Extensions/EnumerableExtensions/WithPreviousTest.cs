using System.Collections.Immutable;
using Funcky.Test.TestUtils;

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
            FunctionalAssert.IsNone(value.Previous);
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
        var length = 200;
        var nonEnumerableList = new FailOnEnumerationList(length);
        var listWithLast = nonEnumerableList.WithPrevious();

        Assert.Equal(137, listWithLast.ElementAt(137).Value);

        Assert.Equal(0, listWithLast.ElementAt(0).Value);
        FunctionalAssert.IsNone(listWithLast.ElementAt(0).Previous);

        foreach (var index in Enumerable.Range(1, length - 1))
        {
            CheckValues(listWithLast, index);
        }
    }

    [Fact]
    public void OptimizedSourceWithIndexCanBeEnumerated()
    {
        var length = 222;
        var nonEnumerableList = Enumerable.Range(0, length).ToList();

        Assert.Equal(length, nonEnumerableList.WithPrevious().Aggregate(0, (sum, _) => sum + 1));
    }

    private static void CheckValues(IEnumerable<ValueWithPrevious<int>> listWithLast, int index)
    {
        Assert.Equal(index, listWithLast.ElementAt(index).Value);

        var previous = FunctionalAssert.IsSome(listWithLast.ElementAt(index).Previous);
        Assert.Equal(index - 1, previous);
    }
}
