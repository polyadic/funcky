#if MIN_MAX_BY
using FsCheck;
using FsCheck.Xunit;

namespace Funcky.Test.Extensions.EnumerableExtensions;

public sealed class MinByOrNoneTest
{
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
    public Property MinByOrNoneWithCustomComparererReturnsTheSameAsMinByWithTheSameCustomeComparer(List<MyRecord> list)
    {
        IComparer<int> customComparer = new CustomIntComparer();
        var minOrNull = list.MinBy(element => element.Number, customComparer);
        var minOrNone = list.MinByOrNone(element => element.Number, customComparer);

        return minOrNone.Match(
                none: () => minOrNull is null,
                some: min => min == minOrNull)
            .ToProperty();
    }

    [Fact]
    public void MinByOrNoneDoesNotThrowOnAnEmptyListOfValueType()
    {
        var emptyList = new List<MyRecordStruct>();

        FunctionalAssert.None(emptyList.MinByOrNone(element => element.Number));
    }

    public record struct MyRecordStruct(int Number, string Text);

    public sealed record MyRecord(int Number, string Text);

    private sealed class CustomIntComparer : IComparer<int>
    {
        public int Compare(int x, int y)
            => Math.Abs(x) - Math.Abs(y);
    }
}
#endif
