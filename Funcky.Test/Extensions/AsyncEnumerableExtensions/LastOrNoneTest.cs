#if INTEGRATED_ASYNC
using static Funcky.Async.Test.Extensions.AsyncEnumerableExtensions.TestData;

namespace Funcky.Async.Test.Extensions.AsyncEnumerableExtensions;

public sealed class LastOrNoneTest
{
    [Fact]
    public async Task LastOrNoneReturnsNoneWhenEnumerableIsEmpty()
    {
        FunctionalAssert.None(await EmptyEnumerable.LastOrNoneAsync());
    }

    [Fact]
    public async Task LastOrNoneReturnsItemWhenEnumerableHasOneItem()
    {
        FunctionalAssert.Some(
            FirstItem,
            await EnumerableWithOneItem.LastOrNoneAsync());
    }

    [Fact]
    public async Task LastOrNoneReturnsNoneWhenEnumerableHasOneItemButItDoesNotMatchPredicate()
    {
        FunctionalAssert.None(
            await EnumerableWithOneItem.LastOrNoneAsync(False));
    }

    [Fact]
    public async Task LastOrNoneReturnsLastItemWhenEnumerableHasMoreThanOneItem()
    {
        FunctionalAssert.Some(
            LastItem,
            await EnumerableWithMoreThanOneItem.LastOrNoneAsync());
    }

    [Fact]
    public async Task LastOrNoneReturnsNoneWhenEnumerableHasItemsButNoneMatchesPredicate()
    {
        FunctionalAssert.None(await EnumerableWithMoreThanOneItem.LastOrNoneAsync(False));
    }
}
#endif
