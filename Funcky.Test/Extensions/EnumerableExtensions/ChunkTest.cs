using Funcky.Test.TestUtils;

namespace Funcky.Test.Extensions.EnumerableExtensions;

public sealed class ChunkTest
{
    [Fact]
    public void ChunkIsEnumeratedLazily()
    {
        var doNotEnumerate = new FailOnEnumerationSequence<object>();

        _ = doNotEnumerate.Chunk(42);
    }

    [Fact]
    public void GivenAnEmptyEnumerableChunkReturnsAnEmptyList()
    {
        var numbers = Enumerable.Empty<int>();

        var chunked = numbers.Chunk(3);

        Assert.Empty(chunked);
    }

    [Fact]
    public void GivenAnSingleElementListWeGetEnumerableWithOneElement()
    {
        var numbers = Sequence.Return(1);

        var chunked = numbers.Chunk(3);

        Assert.Collection(
            chunked,
            chunk =>
            {
                Assert.Collection(
                    chunk,
                    value => Assert.Equal(1, value));
            });
    }

    [Fact]
    public void GivenAnEnumerableWeChanChunkItIntoAnEnumerableOfEnumerables()
    {
        var numbers = Sequence.Return(1, 2, 3, 4, 5, 6, 7, 8, 9);

        var chunked = numbers.Chunk(3);

        Assert.Collection(
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
    public void GivenAnEnumerableNotAMultipleOfSizeWeHaveASmallerLastSlice()
    {
        var numbers = Sequence.Return("a", "b", "c", "d", "e", "g", "h", "i", "j").ToList();

        const int chunkSize = 4;
        #pragma warning disable IDE0007 // False positive
        IEnumerable<IReadOnlyList<string>> chunked = numbers.Chunk(chunkSize);
        #pragma warning restore IDE0007

        Assert.Collection(
            chunked,
            chunk => Assert.Equal(chunkSize, chunk.Count),
            chunk => Assert.Equal(chunkSize, chunk.Count),
            chunk => Assert.Equal(numbers.Count % chunkSize, chunk.Count));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-42)]
    public void ChunkThrowsOnZeroOrNegativeChunkSizes(int invalidChunkSize)
    {
        var numbers = Sequence.Return(1);

        Assert.Throws<ArgumentOutOfRangeException>(() => Funcky.Extensions.EnumerableExtensions.Chunk(numbers, invalidChunkSize));
    }

    [Fact]
    public void ChunkWithResultSelectorAppliesTheSelectorCorrectlyToTheSubsequence()
    {
        var magicSquare = Sequence.Return(4, 9, 2, 3, 5, 7, 8, 1, 6);

        magicSquare
            .Chunk(3, Enumerable.Average)
            .ForEach(average => Assert.Equal(5, average));
    }
}
