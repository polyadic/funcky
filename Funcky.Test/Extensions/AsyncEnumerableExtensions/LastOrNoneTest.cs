#if ASYNC_SUPPORTED
using static Funcky.Async.Test.Extensions.AsyncEnumerableExtensions.TestData;

namespace Funcky.Async.Test.Extensions.AsyncEnumerableExtensions;

public sealed class LastOrNoneTest
{
    [Fact]
    public async Task LastOrNoneReturnsNoneWhenEnumerableIsEmpty()
    {
        Assert.None(await EmptyEnumerable.LastOrNoneAsync());
    }

    [Fact]
    public async Task LastOrNoneReturnsItemWhenEnumerableHasOneItem()
    {
        Assert.Some(
            FirstItem,
            await EnumerableWithOneItem.LastOrNoneAsync());
    }

    [Fact]
    public async Task LastOrNoneReturnsNoneWhenEnumerableHasOneItemButItDoesNotMatchPredicate()
    {
        Assert.None(
            await EnumerableWithOneItem.LastOrNoneAsync(False));
    }

    [Fact]
    public async Task LastOrNoneReturnsLastItemWhenEnumerableHasMoreThanOneItem()
    {
        Assert.Some(
            LastItem,
            await EnumerableWithMoreThanOneItem.LastOrNoneAsync());
    }

    [Fact]
    public async Task LastOrNoneReturnsNoneWhenEnumerableHasItemsButNoneMatchesPredicate()
    {
        Assert.None(await EnumerableWithMoreThanOneItem.LastOrNoneAsync(False));
    }
}
#endif
