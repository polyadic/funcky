#if RANGE_SUPPORTED
using FsCheck;
using FsCheck.Xunit;

namespace Funcky.Test.Extensions;

public sealed class RangeExtensionTest
{
    [Fact]
    public void ThereIsAMatchingSelectManyForAllCombinationsOfRangeAndEnumerable()
    {
        var enumerable = new List<int> { 0, 1, 2 };
        var expected = ExpectedFromEnumerable(enumerable).ToList();

        var rangesOnly = from x in 0..3
                         from y in 0..3
                         select x + y;

        var firstRange = from x in 0..3
                         from y in enumerable
                         select x + y;

        var lastRange = from x in enumerable
                        from y in 0..3
                        select x + y;

        Assert.Equal(expected, rangesOnly);
        Assert.Equal(expected, firstRange);
        Assert.Equal(expected, lastRange);
    }

    [Fact]
    public void YouCanUseARangeInForeachSyntax()
    {
        var expected = new List<int> { 2, 3, 4, 5, 6 };
        var list = new List<int>();

        foreach (var index in 2..7)
        {
            list.Add(index);
        }

        Assert.Equal(expected, list);
    }

    [Fact]
    public void ADownToRangeWorksAsExpected()
    {
        var expected = new List<int> { 11, 10, 9, 8, 7, 6, 5, 4, 3 };
        var list = new List<int>();

        foreach (var index in 11..2)
        {
            list.Add(index);
        }

        Assert.Equal(expected, list);
    }

    [Fact]
    public void RangeExtensionsDoNotSupportEndOfSyntax()
    {
        Assert.Throws<ArgumentException>(ForEachWithEndOfSyntax);
        Assert.Throws<ArgumentException>(SelectWithImplicitEndOfSyntax);
    }

    [Property]
    public Property AnyRangeProducedAValidEnumeration(NonNegativeInt start, NonNegativeInt end)
    {
        var range = new Range(new Index(start.Get), new Index(end.Get));

        var fromRange = from index in range select index;

        return fromRange.SequenceEqual(CreateRange(range)).ToProperty();
    }

    private static IEnumerable<int> ExpectedFromEnumerable(List<int> enumerable)
        => from x in enumerable
           from y in enumerable
           select x + y;

    private static IEnumerable<int> CreateRange(Range range)
        => ToInt(range.Start) < ToInt(range.End)
            ? Enumerable.Range(ToInt(range.Start), GetDistance(range))
            : Enumerable.Range(ToInt(range.End) + 1, GetDistance(range)).Reverse();

    private static int GetDistance(Range range)
        => GetDistance(range.Start, range.End);

    private static int GetDistance(Index start, Index end)
        => Math.Abs(ToInt(end) - ToInt(start));

    private static int ToInt(Index index)
        => index.Value;

    private static void ForEachWithEndOfSyntax()
    {
        foreach (var index in ^11..2)
        {
            _ = index;
        }
    }

    private static IEnumerable<int> SelectWithImplicitEndOfSyntax()
        => (from x in 2.. select x).ToList();
}
#endif
