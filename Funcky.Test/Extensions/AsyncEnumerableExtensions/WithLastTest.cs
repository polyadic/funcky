using Funcky.Test.TestUtils;

namespace Funcky.Test.Extensions.AsyncEnumerableExtensions;

public sealed class WithLastTest
{
    [Fact]
    public void WithLastIsEnumeratedLazily()
    {
        var doNotEnumerate = new FailOnEnumerateAsyncSequence<object>();

        _ = doNotEnumerate.WithLast();
    }

    [Fact]
    public async Task AnEmptySequenceWithLastReturnsAnEmptySequence()
    {
        var emptySequence = AsyncEnumerable.Empty<string>();

        await AsyncAssert.Empty(emptySequence.WithIndex());
    }

    [Fact]
    public async Task ASequenceWithOneElementWithFirstHasOneElementWhichIsMarkedLast()
    {
        const string expectedValue = "Hello world!";
        var oneElementSequence = AsyncSequence.Return(expectedValue);

        var sequenceWithLast = oneElementSequence.WithLast();
        await foreach (var (value, isLast) in sequenceWithLast)
        {
            Assert.Equal(expectedValue, value);
            Assert.True(isLast);
        }

        await AsyncAssert.NotEmpty(sequenceWithLast);
    }

    [Fact]
    public async Task ASequenceWithMultipleElementsWithLastMarksTheLastElement()
    {
        const int length = 20;
        var sequence = AsyncEnumerable.Range(1, length);

        await foreach (var (value, isLast) in sequence.WithLast())
        {
            Assert.Equal(value == length, isLast);
        }
    }
}
