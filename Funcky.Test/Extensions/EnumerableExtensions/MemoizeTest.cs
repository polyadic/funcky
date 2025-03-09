using Funcky.Test.TestUtils;

namespace Funcky.Test;

public sealed class MemoizeTest
{
    [Fact]
    public void MemoizeIsEnumeratedLazily()
    {
        var doNotEnumerate = new FailOnEnumerationSequence<object>();

        using var memoized = doNotEnumerate.Memoize();
    }

    [Fact]
    public void TheUnderlyingEnumerableIsOnlyEnumeratedOnce()
    {
        var enumerateOnce = EnumerateOnce.Create(Sequence.Return("Alpha", "Beta"));
        using var memoized = enumerateOnce.Memoize();

        Assert.Equal("Alpha", memoized.First());
        Assert.Equal("Alpha", memoized.First());

        Assert.Equal("Beta", memoized.Last());
        Assert.Equal("Beta", memoized.Last());
    }

    [Fact]
    public void MemoizingAnEmptyListIsEmpty()
    {
        var empty = Enumerable.Empty<string>().PreventLinqOptimizations();
        using var memoized = empty.Memoize();

        Assert.Empty(memoized);
    }

    [Fact]
    public void OutOfOrderMoveNextReturnsItemsInTheRightOrder()
    {
        using var memoizedRange = Enumerable.Range(1, 10).Memoize();

        using var enumerator1 = memoizedRange.GetEnumerator();

        Assert.True(enumerator1.MoveNext());
        Assert.True(enumerator1.MoveNext());

        Assert.Equal(2, enumerator1.Current);

        using var enumerator2 = memoizedRange.GetEnumerator();

        Assert.True(enumerator2.MoveNext());
        Assert.Equal(1, enumerator2.Current);

        Assert.True(enumerator1.MoveNext());
        Assert.True(enumerator1.MoveNext());

        Assert.Equal(4, enumerator1.Current);

        Assert.True(enumerator2.MoveNext());
        Assert.Equal(2, enumerator2.Current);
        Assert.True(enumerator2.MoveNext());
        Assert.Equal(3, enumerator2.Current);
        Assert.True(enumerator2.MoveNext());
        Assert.Equal(4, enumerator2.Current);
        Assert.True(enumerator2.MoveNext());
        Assert.Equal(5, enumerator2.Current);
    }

    [Fact]
    public void DisposingAMemoizedBufferDoesNotDisposeOriginalBuffer()
    {
        var source = EnumerateOnce.Create<int>([]);
        using var firstMemoization = source.Memoize();

        using (firstMemoization.Memoize())
        {
        }

        firstMemoization.ForEach(NoOperation);
    }

    [Fact]
    public void MemoizingAMemoizedBufferTwiceReturnsTheOriginalObject()
    {
        var source = EnumerateOnce.Create<int>([]);
        using var memoized = source.Memoize();
        using var memoizedBuffer = memoized.Memoize();
        using var memoizedBuffer2 = memoizedBuffer.Memoize();
        Assert.Same(memoizedBuffer, memoizedBuffer2);
    }

    [Fact]
    public void MemoizingAListReturnsAnObjectImplementingIList()
    {
        var source = new List<int> { 10, 20, 30 };
        using var memoized = source.Memoize();
        Assert.IsType<IList<int>>(memoized, exactMatch: false);
    }

    [Fact]
    public void MemoizingACollectionReturnsAnObjectImplementingICollection()
    {
        var source = new HashSet<int> { 10, 20, 30 };
        using var memoized = source.Memoize();
        Assert.IsType<ICollection<int>>(memoized, exactMatch: false);
    }
}
