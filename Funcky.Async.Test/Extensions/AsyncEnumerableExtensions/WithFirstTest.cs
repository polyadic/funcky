using Funcky.Async.Extensions;
using Funcky.Async.Test.TestUtilities;

namespace Funcky.Async.Test.Extensions.AsyncEnumerableExtensions;

public sealed class WithFirstTest
{
    [Fact]
    public void WithFirstIsEnumeratedLazily()
    {
        var doNotEnumerate = new FailOnEnumerateAsyncSequence<object>();

        _ = doNotEnumerate.WithFirst();
    }

    [Fact]
    public async Task AnEmptySequenceWithFirstReturnsAnEmptySequence()
    {
        var emptySequence = AsyncEnumerable.Empty<string>();

        await AsyncAssert.Empty(emptySequence.WithIndex());
    }

    [Fact]
    public async Task ASequenceWithOneElementWithFirstHasOneElementWhichIsMarkedFirst()
    {
        const string expectedValue = "Hello world!";
        var oneElementSequence = AsyncSequence.Return(expectedValue);

        var sequenceWithFirst = oneElementSequence.WithFirst();
        await foreach (var (value, isFirst) in sequenceWithFirst)
        {
            Assert.Equal(expectedValue, value);
            Assert.True(isFirst);
        }

        await AsyncAssert.NotEmpty(sequenceWithFirst);
    }

    [Fact]
    public async Task ASequenceWithMultipleElementsWithFirstMarksTheFirstElement()
    {
        const int length = 20;

        var sequence = AsyncEnumerable.Range(1, length);

        await foreach (var valueWithFirst in sequence.WithFirst())
        {
            Assert.Equal(valueWithFirst.Value == 1, valueWithFirst.IsFirst);
        }
    }
}
