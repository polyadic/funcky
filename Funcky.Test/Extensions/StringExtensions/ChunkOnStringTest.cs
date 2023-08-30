using FsCheck;
using FsCheck.Xunit;

namespace Funcky.Test.Extensions.StringExtensions;

public class ChunkOnStringTest
{
    [Property]
    public Property ChunkingAnEmptyStringWillAlwaysYieldAndEmptySequence(PositiveInt width)
        => string.Empty
            .Chunk(width.Get)
            .None()
            .ToProperty();

    [Fact]
    public void GivenAnSingleElementListWeGetEnumerableWithOneElement()
    {
        var value = "a";

        var chunked = value.Chunk(3);

        Assert.Collection(
            chunked,
            chunk => Assert.Equal("a", chunk));
    }

    [Fact]
    public void GivenAnStringWhichIsNotAMultipleOfSizeWeHaveASmallerLastString()
    {
        var value = "abcdefghij";

        const int chunkSize = 4;
        var chunked = value.Chunk(chunkSize);

        Assert.Collection(
            chunked,
            chunk => Assert.Equal(chunkSize, chunk.Length),
            chunk => Assert.Equal(chunkSize, chunk.Length),
            chunk => Assert.Equal(value.Length % chunkSize, chunk.Length));
    }

    [Fact]
    public void AChunkOfAStringReturnsAListOfConsecutivePartialStrings()
    {
        const int width = 3;
        var source = "epsilon";

        Assert.Collection(
            source.Chunk(width),
            chunk => { Assert.Equal("eps", chunk); },
            chunk => { Assert.Equal("ilo", chunk); },
            chunk => { Assert.Equal("n", chunk); });
    }
}
