namespace Funcky.Async.Test.Extensions.AsyncEnumerableExtensions;

public sealed class PartitionTest
{
    [Fact]
    public async Task ReturnsTwoEmptyListsWhenSourceIsEmpty()
    {
        var (evens, odds) = await AsyncEnumerable.Empty<int>().PartitionAsync(IsEven);
        Assert.Empty(evens);
        Assert.Empty(odds);
    }

    [Fact]
    public async Task PartitionsItemsIntoTruesAndFalses()
    {
        var (evens, odds) = await AsyncEnumerable.Range(0, 6).PartitionAsync(IsEven);
        Assert.Equal([0, 2, 4], evens);
        Assert.Equal([1, 3, 5], odds);
    }

    [Fact]
    public async Task PartitionsItemsIntoTruesAndFalsesWithAsyncPredicate()
    {
        var (evens, odds) = await AsyncEnumerable.Range(0, 6).PartitionAwaitAsync(x => ValueTask.FromResult(IsEven(x)));
        Assert.Equal([0, 2, 4], evens);
        Assert.Equal([1, 3, 5], odds);
    }

    [Fact]
    public async Task PartitionsItemsIntoTruesAndFalsesWithAsyncCancellablePredicate()
    {
        var (evens, odds) = await AsyncEnumerable.Range(0, 6).PartitionAwaitWithCancellationAsync((x, _) => ValueTask.FromResult(IsEven(x)));
        Assert.Equal([0, 2, 4], evens);
        Assert.Equal([1, 3, 5], odds);
    }

    [Fact]
    public async Task RightItemsAreEmptyWhenPredicateMatchesAllItems()
    {
        var (left, right) = await AsyncEnumerable.Range(0, 6).PartitionAsync(True);
        Assert.Equal([0, 1, 2, 3, 4, 5], left);
        Assert.Empty(right);
    }

    [Fact]
    public async Task LeftItemsAreEmptyWhenPredicateMatchesNoItems()
    {
        var (left, right) = await AsyncEnumerable.Range(0, 6).PartitionAsync(False);
        Assert.Empty(left);
        Assert.Equal([0, 1, 2, 3, 4, 5], right);
    }

    private static bool IsEven(int n) => n % 2 == 0;
}
