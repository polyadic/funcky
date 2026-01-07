using Funcky.Test.TestUtils;

namespace Funcky.Test.Extensions.AsyncEnumerableExtensions;

public sealed class WithIndexTest
{
    [Fact]
    public void WithIndexIsEnumeratedLazily()
    {
        var doNotEnumerate = new FailOnEnumerateAsyncSequence<object>();

        _ = doNotEnumerate.WithIndex();
    }

    [Fact]
    public async Task AnEmptySequenceWithIndexReturnsAnEmptySequence()
    {
        var emptySequence = AsyncEnumerable.Empty<string>();

        await AsyncAssert.Empty(emptySequence.WithIndex());
    }

    [Fact]
    public async Task ASequenceWithOneElementWithIndexHasTheIndexZero()
    {
        const string expectedValue = "Hello world!";
        var oneElementSequence = AsyncSequence.Return(expectedValue);

        var sequenceWithIndex = oneElementSequence.WithIndex();
        await foreach (var (value, index) in sequenceWithIndex)
        {
            Assert.Equal(expectedValue, value);
            Assert.Equal(0, index);
        }

        await AsyncAssert.NotEmpty(sequenceWithIndex);
    }

    [Fact]
    public async Task ASequenceWithMultipleElementsWithIndexHaveAscendingIndices()
    {
        var sequence = AsyncEnumerable.Range(0, 20);

        await foreach (var valueWithIndex in sequence.WithIndex())
        {
            Assert.Equal(valueWithIndex.Value, valueWithIndex.Index);
        }
    }
}
