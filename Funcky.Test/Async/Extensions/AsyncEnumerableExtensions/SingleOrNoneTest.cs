using static Funcky.Test.Async.Extensions.AsyncEnumerableExtensions.TestData;

namespace Funcky.Test.Async.Extensions.AsyncEnumerableExtensions;

public sealed class SingleOrNoneTest
{
    [Fact]
    public async Task SingleOrNoneReturnsNoneWhenEnumerableIsEmpty()
    {
        FunctionalAssert.None(await EmptyEnumerable.SingleOrNoneAsync());
    }

    [Fact]
    public async Task SingleOrNoneReturnsItemWhenEnumerableHasOneItem()
    {
        FunctionalAssert.Some(FirstItem, await EnumerableWithOneItem.SingleOrNoneAsync());
    }

    [Fact]
    public async Task SingleOrNoneThrowsWhenEnumerableContainsMoreThanOneItem()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await EnumerableWithMoreThanOneItem.SingleOrNoneAsync());
    }

    [Fact]
    public async Task SingleOrNoneReturnsNoneWhenEnumerableHasOneItemAndPredicateDoesNotMatch()
    {
        FunctionalAssert.None(await EnumerableWithOneItem.SingleOrNoneAsync(False));
    }

    [Fact]
    public async Task SingleOrNoneReturnsItemWhenEnumerableHasOneAndPredicateMatches()
    {
        FunctionalAssert.Some(FirstItem, await EnumerableWithOneItem.SingleOrNoneAsync(True));
    }

    [Fact]
    public async Task SingleOrNoneReturnsItemWhenEnumerableContainsMoreThanOneItemButOnlyOneMatchesPredicate()
    {
        FunctionalAssert.Some(FirstItem, await EnumerableWithMoreThanOneItem.SingleOrNoneAsync(item => item == FirstItem));
    }

    [Fact]
    public async Task SingleOrNoneThrowsWhenEnumerableContainsMoreThanOneMatchingItem()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await EnumerableWithMoreThanOneItem.SingleOrNoneAsync(True));
    }

    [Fact]
    public async Task SingleOrNoneOnlyAdvancesIteratorUpToMatchingItemAndNextBeforeThrowing()
    {
        const int matchingItem = 3;
        const int itemAfterMatchingItem = matchingItem + 1;
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await NumbersGreaterThanOrEqualToZero(throwExceptionWhenValueGreaterThan: itemAfterMatchingItem)
                .SingleOrNoneAsync(n => n >= matchingItem));
    }

    private static async IAsyncEnumerable<int> NumbersGreaterThanOrEqualToZero(int throwExceptionWhenValueGreaterThan)
    {
        for (var value = 0; ; value++)
        {
            if (value > throwExceptionWhenValueGreaterThan)
            {
                throw new NotSupportedException();
            }

            yield return await Task.FromResult(value);
        }
    }
}
