#if NET6_0_OR_GREATER
using FsCheck;
using FsCheck.Xunit;
#endif

namespace Funcky.Test.Extensions.EnumerableExtensions;

public sealed class MaxByOrNoneTest
{
#if NET6_0_OR_GREATER
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
    public Property MaxByOrNoneWithCustomComparerReturnsTheSameAsMaxByWithTheSameCustomComparer(List<MyRecord> list)
    {
        IComparer<int> customComparer = new CustomIntComparer();
        var maxOrNull = list.MaxBy(element => element.Number, customComparer);
        var maxOrNone = list.MaxByOrNone(element => element.Number, customComparer);

        return maxOrNone.Match(
                none: () => maxOrNull is null,
                some: max => max == maxOrNull)
            .ToProperty();
    }
#endif

    [Fact]
    public void MaxByOrNoneReturnsNoneOnAnEmptyList()
    {
        var emptyList = new List<MyRecord>();

        FunctionalAssert.None(emptyList.MaxByOrNone(element => element.Number));
    }

    [Fact]
    public void MaxByOrNoneDoesNotThrowOnAnEmptyListOfValueType()
    {
        var emptyList = new List<MyRecordStruct>();

        FunctionalAssert.None(emptyList.MaxByOrNone(element => element.Number));
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
