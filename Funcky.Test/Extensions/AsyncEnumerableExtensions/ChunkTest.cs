#if INTEGRATED_ASYNC
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
        var canceledToken = new CancellationToken(canceled: true);
        _ = await new AssertIsCancellationRequestedAsyncSequence<Unit>().Chunk(1, Enumerable.Count).ToListAsync(canceledToken);
    }
}
#endif
