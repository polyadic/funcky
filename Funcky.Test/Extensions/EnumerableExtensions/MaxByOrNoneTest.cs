#if MIN_MAX_BY
using FsCheck;
using FsCheck.Xunit;

namespace Funcky.Test.Extensions.EnumerableExtensions;

public sealed class MaxByOrNoneTest
{
    [Property]
    public Property MaxByOrNoneReturnsTheSameAsMaxBy(List<MyRecord> list)
    {
        var maxOrNull = list.MaxBy(element => element.Number);
        var maxOrNone = list.MaxByOrNone(element => element.Number);

        return maxOrNone.Match(
                none: () => maxOrNull is null,
                some: max => max == maxOrNull)
            .ToProperty();
    }

    [Property]
    public Property MaxByOrNoneWithCustomComparererReturnsTheSameAsMaxByWithTheSameCustomeComparer(List<MyRecord> list)
    {
        IComparer<int> customComparer = new CustomIntComparer();
        var maxOrNull = list.MaxBy(element => element.Number, customComparer);
        var maxOrNone = list.MaxByOrNone(element => element.Number, customComparer);

        return maxOrNone.Match(
                none: () => maxOrNull is null,
                some: max => max == maxOrNull)
            .ToProperty();
    }

    [Fact]
    public void MaxByOrNoneDoesNotThrowOnAnEmptyListOfValueType()
    {
        var emptyList = new List<MyRecordStruct>();

        FunctionalAssert.None(emptyList.MaxByOrNone(element => element.Number));
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
