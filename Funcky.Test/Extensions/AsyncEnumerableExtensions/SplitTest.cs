#if ASYNC_SUPPORTED
using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using Funcky.Async.Test.TestUtilities;

namespace Funcky.Async.Test.Extensions.AsyncEnumerableExtensions;

public sealed class SplitTest
{
    [Fact]
    public void SplitIsAnIAsyncEnumerableLazily()
    {
        var doNotEnumerate = new FailOnEnumerateAsyncSequence<object>();

        _ = doNotEnumerate.Split(42);
    }

    [Property]
    public Property SplittingAnEmptyIAsyncEnumerableReturnsASingleEmptyPart(int separator)
    {
        var parts = AsyncEnumerable.Empty<int>().Split(separator).ToListAsync().Result;

        return (parts.Count == 1 && parts[0].Count == 0).ToProperty();
    }

    [Property]
    public Property SplitBehavesLikeStringSplit(List<char> characters, char separator)
    {
        var expected = new string(characters.ToArray()).Split(separator);
        var parts = characters.ToAsyncEnumerable().Split(separator).ToListAsync().Result.Select(part => new string(part.ToArray()));

        return expected.SequenceEqual(parts).ToProperty();
    }

    [Fact]
    public async Task ASeparatorAtTheEndProducesATrailingEmptyPart()
    {
        var parts = await AsyncSequence.Return(1, 0).Split(0).ToListAsync();

        Assert.Equal([[1], []], parts.Select(part => part.ToList()));
    }

    [Fact]
    public async Task ALoneSeparatorProducesTwoEmptyParts()
    {
        var parts = await AsyncSequence.Return(0).Split(0).ToListAsync();

        Assert.Equal([[], []], parts.Select(part => part.ToList()));
    }

    [Fact]
    public async Task SplitAnIAsyncEnumerableCorrectly()
    {
        var sequence = AsyncSequence.Return(12, 14, 7, 41, 31, 19, 7, 9, 11, 99, 99);

        var parts = sequence.Split(7);

        var expected = AsyncSequence.Return(
            Sequence.Return(12, 14),
            Sequence.Return(41, 31, 19),
            Sequence.Return(9, 11, 99, 99));

        await AsyncAssert.Equal(expected, parts);
    }
}
#endif
