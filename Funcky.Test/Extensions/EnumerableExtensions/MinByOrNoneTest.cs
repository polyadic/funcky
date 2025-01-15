#if NET6_0_OR_GREATER
using FsCheck;
using FsCheck.Xunit;
#endif

namespace Funcky.Test.Extensions.EnumerableExtensions;

public sealed class MinByOrNoneTest
{
#if NET6_0_OR_GREATER
    [Property]
    public Property MinByOrNoneReturnsTheSameAsMinBy(List<MyRecord> list)
    {
        var minOrNull = list.MinBy(element => element.Number);
        var minOrNone = list.MinByOrNone(element => element.Number);

        return minOrNone.Match(
            none: () => minOrNull is null,
            some: min => min == minOrNull)
            .ToProperty();
    }

    [Property]
    public Property MinByOrNoneWithCustomComparererReturnsTheSameAsMinByWithTheSameCustomComparer(List<MyRecord> list)
    {
        IComparer<int> customComparer = new CustomIntComparer();
        var minOrNull = list.MinBy(element => element.Number, customComparer);
        var minOrNone = list.MinByOrNone(element => element.Number, customComparer);

        return minOrNone.Match(
                none: () => minOrNull is null,
                some: min => min == minOrNull)
            .ToProperty();
    }
#endif

    [Fact]
    public void MinByOrNoneReturnsNoneOnAnEmptyList()
    {
        var emptyList = new List<MyRecord>();

        FunctionalAssert.None(emptyList.MinByOrNone(element => element.Number));
    }

    [Fact]
    public void MinByOrNoneDoesNotThrowOnAnEmptyListOfValueType()
    {
        var emptyList = new List<MyRecordStruct>();

        FunctionalAssert.None(emptyList.MinByOrNone(element => element.Number));
    }

    public record struct MyRecordStruct(int Number, string Text);

    public sealed record MyRecord(int Number, string Text);

#if NET6_0_OR_GREATER
    private sealed class CustomIntComparer : IComparer<int>
    {
        public int Compare(int x, int y)
            => Math.Abs(x) - Math.Abs(y);
    }
#endif
}
