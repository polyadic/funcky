using Funcky.Async.Test.TestUtilities;

namespace Funcky.Async.Test.Extensions.AsyncEnumerableExtensions;

public sealed class ChunkTest
{
    [Fact]
    public void ChunkIsEnumeratedLazily()
    {
        var doNotEnumerate = new FailOnEnumerateAsyncSequence<object>();

        _ = doNotEnumerate.Chunk(42);
    }

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
            a =>
            {
                Assert.Collection(
                    a,
                    aa => Assert.Equal(1, aa),
                    ab => Assert.Equal(2, ab),
                    ac => Assert.Equal(3, ac));
            },
            b =>
            {
                Assert.Collection(
                    b,
                    ba => Assert.Equal(4, ba),
                    bb => Assert.Equal(5, bb),
                    bc => Assert.Equal(6, bc));
            },
            c =>
            {
                Assert.Collection(
                    c,
                    ca => Assert.Equal(7, ca),
                    cb => Assert.Equal(8, cb),
                    cc => Assert.Equal(9, cc));
            });
    }

    [Fact]
    public async Task GivenAnEnumerableNotAMultipleOfSizeWeHaveASmallerLastSlice()
    {
        var numbers = AsyncSequence.Return("a", "b", "c", "d", "e", "g", "h", "i", "j");

        const int chunkSize = 4;
        var chunked = numbers.Chunk(chunkSize);
        var count = await numbers.CountAsync();

        await AsyncAssert.Collection(
            chunked,
            a =>
            {
                Assert.Equal(chunkSize, a.Count);
            },
            b =>
            {
                Assert.Equal(chunkSize, b.Count);
            },
            c =>
            {
                Assert.Equal(count % chunkSize, c.Count);
            });
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-42)]
    public void ChunkThrowsOnZeroOrNegativeChunkSizes(int invalidChunkSize)
    {
        var numbers = AsyncSequence.Return(1);

        Assert.Throws<ArgumentOutOfRangeException>(() => numbers.Chunk(invalidChunkSize));
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
}
