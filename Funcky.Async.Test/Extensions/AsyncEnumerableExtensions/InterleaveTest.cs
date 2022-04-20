using Funcky.Async.Extensions;
using Funcky.Async.Test.TestUtilities;
using Xunit;

namespace Funcky.Async.Test.Extensions.AsyncEnumerableExtensions;

public sealed class InterleaveTest
{
    [Fact]
    public void InterleaveIsEnumeratedLazily()
    {
        var doNotEnumerate = new FailOnEnumerateAsyncSequence<object>();

        _ = doNotEnumerate.Interleave();
    }

    [Fact]
    public async Task GivenAnEmptySequenceOfSequencesInterleaveReturnsAnEmptySequence()
    {
        var emptySequence = Enumerable.Empty<IAsyncEnumerable<int>>();

        var interleaved = emptySequence.Interleave();

        await AsyncAssert.Empty(interleaved);
    }

    [Fact]
    public async Task GivenTwoSequencesOfEqualLengthIGetAnInterleavedResult()
    {
        var odds = AsyncSequence.Return(1, 3, 5, 7, 9, 11);
        var evens = AsyncSequence.Return(2, 4, 6, 8, 10, 12);
        var expected = AsyncSequence.Return(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12);

        var interleaved = odds.Interleave(evens);

        await AsyncAssert.Equal(expected, interleaved);
    }

    [Fact]
    public async Task GivenTwoSequencesOfUnequalLengthIGetAnInterleavedResult()
    {
        var odds = AsyncSequence.Return(1, 3, 5, 7, 9, 11);
        var evens = AsyncSequence.Return(2, 4, 6);
        var expected = AsyncSequence.Return(1, 2, 3, 4, 5, 6, 7, 9, 11);

        var interleaved = odds.Interleave(evens);

        await AsyncAssert.Equal(expected, interleaved);
    }

    [Theory]
    [InlineData("a", "b", "c")]
    [InlineData("a", "c", "b")]
    [InlineData("b", "a", "c")]
    [InlineData("b", "c", "a")]
    [InlineData("c", "a", "b")]
    [InlineData("c", "b", "a")]
    public async Task GivenMultipleSequencesTheOrderIsPreserved(string first, string second, string third)
    {
        var one = AsyncSequence.Return(first);
        var two = AsyncSequence.Return(second);
        var three = AsyncSequence.Return(third);

        var interleaved = one.Interleave(two, three);

        await AsyncAssert.Equal(AsyncSequence.Return(first, second, third), interleaved);
    }

    [Fact]
    public async Task GivenASequenceOfSequenceTheInnerSequencesGetInterleaved()
    {
        var sequences = Sequence.Return(AsyncEnumerable.Repeat(1, 2), AsyncEnumerable.Repeat(42, 2));

        await AsyncAssert.Equal(AsyncSequence.Return(1, 42, 1, 42), sequences.Interleave());
    }

    [Fact]
    public async Task GivenOneSequenceWithElementsAndAllTheOtherSequencesEmptyWeGetTheFirstSequence()
    {
        var sequence = AsyncSequence.Return(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12);
        var emptySequence = AsyncEnumerable.Empty<int>();

        await AsyncAssert.Equal(sequence, emptySequence.Interleave(emptySequence, sequence, emptySequence));
    }

    [Fact]
    public async Task GivenASequenceOfSequencesInterleaveReturnsTheExpectedSequence()
    {
        var sequences = Sequence.Return(AsyncEnumerable.Repeat(1, 10), AsyncEnumerable.Repeat(2, 10), AsyncEnumerable.Repeat(3, 10), AsyncEnumerable.Repeat(4, 10));

        var innerSum = sequences.Select(async element => await element.CountAsync()).Aggregate(0, (total, part) => total + part.Result);
        Assert.Equal(innerSum, await sequences.Interleave().CountAsync());

        int expected = 1;
        await foreach (var element in sequences.Interleave())
        {
            Assert.Equal(expected, element);
            expected = (expected % 4) + 1;
        }
    }
}
