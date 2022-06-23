using Funcky.Async.Test.TestUtilities;

namespace Funcky.Test;

public sealed class MemoizeTest
{
    [Fact]
    public async Task MemoizeIsEnumeratedLazily()
    {
        var doNotEnumerate = new FailOnEnumerateAsyncSequence<object>();

        await using var memoized = doNotEnumerate.Memoize();
    }

    [Fact]
    public async Task TheUnderlyingAsyncEnumerableIsOnlyEnumeratedOnce()
    {
        var enumerateOnce = AsyncEnumerateOnce.Create(Sequence.Return("Alpha", "Beta"));
        await using var memoized = enumerateOnce.Memoize();

        Assert.Equal("Alpha", await memoized.FirstAsync());
        Assert.Equal("Alpha", await memoized.FirstAsync());

        Assert.Equal("Beta", await memoized.LastAsync());
        Assert.Equal("Beta", await memoized.LastAsync());
    }

    [Fact]
    public async Task MemoizingAnEmptyListIsEmptyAsync()
    {
        var empty = AsyncEnumerable.Empty<string>();
        await using var memoized = empty.Memoize();

        await AsyncAssert.Empty(memoized);
    }

    [Fact]
    public async Task OutOfOrderMoveNextReturnsItemsInTheRightOrderAsync()
    {
        await using var memoizedRange = AsyncEnumerable.Range(1, 10).Memoize();

        await using var enumerator1 = memoizedRange.GetAsyncEnumerator();

        Assert.True(await enumerator1.MoveNextAsync());
        Assert.True(await enumerator1.MoveNextAsync());

        Assert.Equal(2, enumerator1.Current);

        await using var enumerator2 = memoizedRange.GetAsyncEnumerator();

        Assert.True(await enumerator2.MoveNextAsync());
        Assert.Equal(1, enumerator2.Current);

        Assert.True(await enumerator1.MoveNextAsync());
        Assert.True(await enumerator1.MoveNextAsync());

        Assert.Equal(4, enumerator1.Current);

        Assert.True(await enumerator2.MoveNextAsync());
        Assert.Equal(2, enumerator2.Current);
        Assert.True(await enumerator2.MoveNextAsync());
        Assert.Equal(3, enumerator2.Current);
        Assert.True(await enumerator2.MoveNextAsync());
        Assert.Equal(4, enumerator2.Current);
        Assert.True(await enumerator2.MoveNextAsync());
        Assert.Equal(5, enumerator2.Current);
    }
}
