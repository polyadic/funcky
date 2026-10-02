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
    public Property SplittingAnEmptyEnumerableAlwaysReturnsAnEmptyEnumerable(int separator)
    {
        var parts = Enumerable.Empty<int>().Split(separator);

        return (!parts.Any()).ToProperty();
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
