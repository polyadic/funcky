#if ASYNC_SUPPORTED
using static Funcky.Async.Test.Extensions.AsyncEnumerableExtensions.TestData;

namespace Funcky.Async.Test.Extensions.AsyncEnumerableExtensions;

public sealed class FirstOrNoneTest
{
    [Fact]
    public async Task FirstOrNoneReturnsNoneWhenEnumerableIsEmpty()
    {
        Assert.None(await EmptyEnumerable.FirstOrNoneAsync());
    }

    [Fact]
    public async Task FirstOrNoneReturnsItemWhenEnumerableHasOneItem()
    {
        Assert.Some(
            FirstItem,
            await EnumerableWithOneItem.FirstOrNoneAsync());
    }

    [Fact]
    public async Task FirstOrNoneReturnsNoneWhenEnumerableHasOneItemButItDoesNotMatchPredicate()
    {
        Assert.None(
            await EnumerableWithOneItem.FirstOrNoneAsync(False));
    }

    [Fact]
    public async Task FirstOrNoneReturnsItemWhenEnumerableHasMoreThanOneItem()
    {
        Assert.Some(
            FirstItem,
            await EnumerableWithMoreThanOneItem.FirstOrNoneAsync());
    }

    [Fact]
    public async Task FirstOrNoneReturnsNoneWhenEnumerableHasItemsButNoneMatchesPredicate()
    {
        Assert.None(await EnumerableWithMoreThanOneItem.FirstOrNoneAsync(False));
    }
}
#endif
