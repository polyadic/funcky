#if ASYNC_SUPPORTED
using Funcky.Async.Test.TestUtilities;

namespace Funcky.Async.Test.Extensions.AsyncEnumerableExtensions;

public sealed class ChunkTest
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-42)]
    public void ChunkThrowsOnZeroOrNegativeChunkSizes(int invalidChunkSize)
    {
        var numbers = AsyncSequence.Return(1);

        Assert.Throws<ArgumentOutOfRangeException>(() => numbers.Chunk(invalidChunkSize, Enumerable.Sum));
    }

    [Fact]
    public async Task ChunkWithResultSelectorAppliesTheSelectorCorrectlyToTheSubsequenceAsync()
    {
        var magicSquare = AsyncSequence.Return(4, 9, 2, 3, 5, 7, 8, 1, 6);

        await foreach (var average in magicSquare.Chunk(3, Enumerable.Average))
        {
            Assert.Equal(5, average);
        }
    }

    [Fact]
    public async Task ChunkAwaitWithResultSelectorAppliesTheSelectorCorrectlyToTheSubsequenceAsync()
    {
        var magicSquare = AsyncSequence.Return(4, 9, 2, 3, 5, 7, 8, 1, 6);

        await foreach (var average in magicSquare.ChunkAwait(3, chunk => ValueTask.FromResult(chunk.Average())))
        {
            Assert.Equal(5, average);
        }
    }

    [Fact]
    public async Task ChunkAwaitWithCancellationWithResultSelectorAppliesTheSelectorCorrectlyToTheSubsequenceAsync()
    {
        var magicSquare = AsyncSequence.Return(4, 9, 2, 3, 5, 7, 8, 1, 6);

        await foreach (var average in magicSquare.ChunkAwaitWithCancellation(3, (chunk, _) => ValueTask.FromResult(chunk.Average())))
        {
            Assert.Equal(5, average);
        }
    }

    [Fact]
    public async Task GivenAnEnumerableNotAMultipleOfSizeWeHaveASmallerLastSlice()
    {
        var numbers = AsyncSequence.Return("a", "b", "c", "d", "e", "g", "h", "i", "j");

        const int chunkSize = 4;
        var chunked = numbers.Chunk(chunkSize, chunk => chunk.Count);
        var count = await numbers.CountAsync();

        await AsyncAssert.Collection(
            chunked,
            chunk => Assert.Equal(chunkSize, chunk),
            chunk => Assert.Equal(chunkSize, chunk),
            chunk => Assert.Equal(count % chunkSize, chunk));
    }

    [Fact]
    public async Task CancellationIsPropagated()
    {
        await AsyncAssert.CancellationIsPropagated(new AssertIsCancellationRequestedAsyncSequence<Unit>().Chunk(1, Enumerable.Count));
    }

#if SYSTEM_LINQ_ASYNC
    // The Chunk(size) overload only exists in Funcky.Async; on .NET 10 the base class library provides it.
    [Fact]
    public async Task GivenAnEmptyEnumerableChunkReturnsAnEmptyListAsync()
    {
        var numbers = AsyncEnumerable.Empty<int>();

        var chunked = numbers.Chunk(3);

        await AsyncAssert.Empty(chunked);
    }

    [Fact]
    public async Task GivenAnSingleElementListWeGetEnumerableWithOneElement()
    {
        var numbers = AsyncSequence.Return(1);

        var chunked = numbers.Chunk(3);
        var expected = AsyncSequence.Return(new List<int> { 1 });

        await AsyncAssert.Equal(expected, chunked);
    }

    [Fact]
    public async Task GivenAnEnumerableWeChanChunkItIntoAnEnumerableOfEnumerablesAsync()
    {
        var numbers = AsyncSequence.Return(1, 2, 3, 4, 5, 6, 7, 8, 9);

        var chunked = numbers.Chunk(3);

        await AsyncAssert.Collection(
            chunked,
            chunk =>
            {
                Assert.Collection(
                    chunk,
                    value => Assert.Equal(1, value),
                    value => Assert.Equal(2, value),
                    value => Assert.Equal(3, value));
            },
            chunk =>
            {
                Assert.Collection(
                    chunk,
                    value => Assert.Equal(4, value),
                    value => Assert.Equal(5, value),
                    value => Assert.Equal(6, value));
            },
            chunk =>
            {
                Assert.Collection(
                    chunk,
                    value => Assert.Equal(7, value),
                    value => Assert.Equal(8, value),
                    value => Assert.Equal(9, value));
            });
    }

    [Fact]
    public async Task GivenAnEnumerableNotAMultipleOfSizeWeHaveASmallerLastSliceWithoutResultSelector()
    {
        var numbers = AsyncSequence.Return("a", "b", "c", "d", "e", "g", "h", "i", "j");

        const int chunkSize = 4;
        var chunked = numbers.Chunk(chunkSize);
        var count = await numbers.CountAsync();

        await AsyncAssert.Collection(
            chunked,
            chunk => Assert.Equal(chunkSize, chunk.Count),
            chunk => Assert.Equal(chunkSize, chunk.Count),
            chunk => Assert.Equal(count % chunkSize, chunk.Count));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-42)]
    public void ChunkWithoutResultSelectorThrowsOnZeroOrNegativeChunkSizes(int invalidChunkSize)
    {
        var numbers = AsyncSequence.Return(1);

        Assert.Throws<ArgumentOutOfRangeException>(() => numbers.Chunk(invalidChunkSize));
    }
#endif
}
#endif
