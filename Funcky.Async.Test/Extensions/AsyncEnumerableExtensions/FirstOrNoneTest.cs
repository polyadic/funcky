using Funcky.Async.Extensions;
using static Funcky.Async.Test.Extensions.AsyncEnumerableExtensions.TestData;

namespace Funcky.Async.Test.Extensions.AsyncEnumerableExtensions;

public sealed class FirstOrNoneTest
{
    [Fact]
    public async Task FirstOrNoneReturnsNoneWhenEnumerableIsEmpty()
    {
        FunctionalAssert.IsNone(await EmptyEnumerable.FirstOrNoneAsync());
    }

    [Fact]
    public async Task FirstOrNoneReturnsItemWhenEnumerableHasOneItem()
    {
        FunctionalAssert.IsSome(
            FirstItem,
            await EnumerableWithOneItem.FirstOrNoneAsync());
    }

    [Fact]
    public async Task FirstOrNoneReturnsNoneWhenEnumerableHasOneItemButItDoesNotMatchPredicate()
    {
        FunctionalAssert.IsNone(
            await EnumerableWithOneItem.FirstOrNoneAsync(False));
    }

    [Fact]
    public async Task FirstOrNoneReturnsItemWhenEnumerableHasMoreThanOneItem()
    {
        FunctionalAssert.IsSome(
            FirstItem,
            await EnumerableWithMoreThanOneItem.FirstOrNoneAsync());
    }

    [Fact]
    public async Task FirstOrNoneReturnsNoneWhenEnumerableHasItemsButNoneMatchesPredicate()
    {
        FunctionalAssert.IsNone(await EnumerableWithMoreThanOneItem.FirstOrNoneAsync(False));
    }
}
