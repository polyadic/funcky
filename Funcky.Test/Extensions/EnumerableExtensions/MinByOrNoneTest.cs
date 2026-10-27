#if NET6_0_OR_GREATER
using FsCheck;
using FsCheck.Fluent;
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

    [Property]
    public Property MinByOrNoneSkipsNullKeysLikeTheBclDoes(List<MyNullableKeyRecord> list)
    {
        var resultOrNull = list.MinBy(element => element.Number);
        var resultOrNone = list.MinByOrNone(element => element.Number);

        return resultOrNone.Match(
                none: () => resultOrNull is null,
                some: result => result == resultOrNull)
            .ToProperty();
    }
#endif

    [Fact]
    public void MinByOrNoneSkipsElementsWithANullKey()
    {
        var list = new List<MyNullableKeyRecord> { new(null, "first"), new(2, "second"), new(null, "third"), new(1, "fourth") };

        Assert.Some(new MyNullableKeyRecord(1, "fourth"), list.MinByOrNone(element => element.Number));
    }

    [Fact]
    public void MinByOrNoneReturnsTheFirstElementWhenAllKeysAreNull()
    {
        var list = new List<MyNullableKeyRecord> { new(null, "first"), new(null, "second") };

        Assert.Some(new MyNullableKeyRecord(null, "first"), list.MinByOrNone(element => element.Number));
    }

    [Fact]
    public void MinByOrNoneEvaluatesTheKeySelectorOnceForEachElement()
    {
        var list = new List<MyRecord> { new(3, "a"), new(1, "b"), new(2, "c") };
        var calls = 0;

        _ = list.MinByOrNone(element =>
        {
            calls++;
            return element.Number;
        });

        Assert.Equal(list.Count, calls);
    }

    [Fact]
    public void MinByOrNoneReturnsNoneOnAnEmptyList()
    {
        var emptyList = new List<MyRecord>();

        Assert.None(emptyList.MinByOrNone(element => element.Number));
    }

    [Fact]
    public void MinByOrNoneDoesNotThrowOnAnEmptyListOfValueType()
    {
        var emptyList = new List<MyRecordStruct>();

        Assert.None(emptyList.MinByOrNone(element => element.Number));
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
