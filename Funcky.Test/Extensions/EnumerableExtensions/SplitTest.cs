using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using Funcky.Test.TestUtilities;

namespace Funcky.Test.Extensions.EnumerableExtensions;

public sealed class SplitTest
{
    [Fact]
    public void SplitIsEnumeratedLazily()
    {
        var doNotEnumerate = new FailOnEnumerationSequence<object>();

        _ = doNotEnumerate.Split(42);
    }

    [Property]
    public Property SplittingAnEmptyEnumerableReturnsASingleEmptyPart(int separator)
    {
        var parts = Enumerable.Empty<int>().Split(separator);

        return (parts.Count() == 1 && !parts.First().Any()).ToProperty();
    }

    [Property]
    public Property SplitBehavesLikeStringSplit(List<char> characters, char separator)
    {
        var expected = new string(characters.ToArray()).Split(separator);
        var parts = characters.Split(separator).Select(part => new string(part.ToArray()));

        return expected.SequenceEqual(parts).ToProperty();
    }

    [Property]
    public Property JoiningThePartsWithTheSeparatorRestoresTheSource(List<int> source, int separator)
    {
        var parts = source.Split(separator);

        return source.SequenceEqual(parts.Intersperse([separator]).SelectMany(Identity)).ToProperty();
    }

    [Fact]
    public void ASeparatorAtTheEndProducesATrailingEmptyPart()
    {
        Assert.Equal([[1], []], Sequence.Return(1, 0).Split(0).Select(part => part.ToList()));
    }

    [Fact]
    public void ASeparatorAtTheStartProducesALeadingEmptyPart()
    {
        Assert.Equal([[], [1]], Sequence.Return(0, 1).Split(0).Select(part => part.ToList()));
    }

    [Fact]
    public void ALoneSeparatorProducesTwoEmptyParts()
    {
        Assert.Equal([[], []], Sequence.Return(0).Split(0).Select(part => part.ToList()));
    }

    [Fact]
    public void SplitAnEnumerableCorrectly()
    {
        var sequence = Sequence.Return(12, 14, 7, 41, 31, 19, 7, 9, 11, 99, 99);

        var parts = sequence.Split(7);

        var expected = Sequence.Return(
            Sequence.Return(12, 14),
            Sequence.Return(41, 31, 19),
            Sequence.Return(9, 11, 99, 99));

        Assert.Equal(expected, parts);
    }

    [Fact]
    public void SplitOnAnArrayBindsToTheEnumerableOverload()
    {
        // With C# 14 first-class span conversions, an array receiver would otherwise bind to MemoryExtensions.Split(ReadOnlySpan<T>, T).
        int[] array = [1, 0, 2, 0, 3];

        var parts = array.Split(0);

        Assert.Equal([[1], [2], [3]], parts.Select(part => part.ToList()));
    }
}
