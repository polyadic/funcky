#if NET6_0_OR_GREATER
using FsCheck;
using FsCheck.Fluent;
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

    [Property]
    public Property MaxByOrNoneSkipsNullKeysLikeTheBclDoes(List<MyNullableKeyRecord> list)
    {
        var resultOrNull = list.MaxBy(element => element.Number);
        var resultOrNone = list.MaxByOrNone(element => element.Number);

        return resultOrNone.Match(
                none: () => resultOrNull is null,
                some: result => result == resultOrNull)
            .ToProperty();
    }
#endif

    [Fact]
    public void MaxByOrNoneSkipsElementsWithANullKey()
    {
        var list = new List<MyNullableKeyRecord> { new(null, "first"), new(2, "second"), new(null, "third"), new(1, "fourth") };

        FunctionalAssert.Some(new MyNullableKeyRecord(2, "second"), list.MaxByOrNone(element => element.Number));
    }

    [Fact]
    public void MaxByOrNoneReturnsTheFirstElementWhenAllKeysAreNull()
    {
        var list = new List<MyNullableKeyRecord> { new(null, "first"), new(null, "second") };

        FunctionalAssert.Some(new MyNullableKeyRecord(null, "first"), list.MaxByOrNone(element => element.Number));
    }

    [Fact]
    public void MaxByOrNoneEvaluatesTheKeySelectorOnceForEachElement()
    {
        var list = new List<MyRecord> { new(3, "a"), new(1, "b"), new(2, "c") };
        var calls = 0;

        _ = list.MaxByOrNone(element =>
        {
            calls++;
            return element.Number;
        });

        Assert.Equal(list.Count, calls);
    }

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

    public sealed record MyNullableKeyRecord(int? Number, string Text);

#if NET6_0_OR_GREATER
    private sealed class CustomIntComparer : IComparer<int>
    {
        public int Compare(int x, int y)
            => Math.Abs(x) - Math.Abs(y);
    }
#endif
}
