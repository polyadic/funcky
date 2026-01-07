using static Funcky.Test.Extensions.AsyncEnumerableExtensions.TestData;

namespace Funcky.Test.Extensions.AsyncEnumerableExtensions;

public sealed class FirstOrNoneTest
{
    [Fact]
    public async Task FirstOrNoneReturnsNoneWhenEnumerableIsEmpty()
    {
        FunctionalAssert.None(await EmptyEnumerable.FirstOrNoneAsync());
    }

    [Fact]
    public async Task FirstOrNoneReturnsItemWhenEnumerableHasOneItem()
    {
        FunctionalAssert.Some(
            FirstItem,
            await EnumerableWithOneItem.FirstOrNoneAsync());
    }

    [Fact]
    public async Task FirstOrNoneReturnsNoneWhenEnumerableHasOneItemButItDoesNotMatchPredicate()
    {
        FunctionalAssert.None(
            await EnumerableWithOneItem.FirstOrNoneAsync(False));
    }

    [Fact]
    public async Task FirstOrNoneReturnsItemWhenEnumerableHasMoreThanOneItem()
    {
        FunctionalAssert.Some(
            FirstItem,
            await EnumerableWithMoreThanOneItem.FirstOrNoneAsync());
    }

    [Fact]
    public async Task FirstOrNoneReturnsNoneWhenEnumerableHasItemsButNoneMatchesPredicate()
    {
        FunctionalAssert.None(await EnumerableWithMoreThanOneItem.FirstOrNoneAsync(False));
    }
}
