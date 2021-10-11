using Funcky.Async.Extensions;
using Xunit;
using static Funcky.Async.Test.Extensions.AsyncEnumerableExtensions.TestData;
using static Funcky.Functional;

namespace Funcky.Async.Test.Extensions.AsyncEnumerableExtensions
{
    public sealed class SingleOrNoneTest
    {
        [Fact]
        public async Task SingleOrNoneReturnsNoneWhenEnumerableIsEmpty()
        {
            FunctionalAssert.IsNone(await EmptyEnumerable.SingleOrNoneAsync());
        }

        [Fact]
        public async Task SingleOrNoneReturnsItemWhenEnumerableHasOneItem()
        {
            FunctionalAssert.IsSome(FirstItem, await EnumerableWithOneItem.SingleOrNoneAsync());
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
            FunctionalAssert.IsNone(await EnumerableWithOneItem.SingleOrNoneAsync(False));
        }

        [Fact]
        public async Task SingleOrNoneReturnsItemWhenEnumerableHasOneAndPredicateMatches()
        {
            FunctionalAssert.IsSome(FirstItem, await EnumerableWithOneItem.SingleOrNoneAsync(True));
        }

        [Fact]
        public async Task SingleOrNoneReturnsItemWhenEnumerableContainsMoreThanOneItemButOnlyOneMatchesPredicate()
        {
            FunctionalAssert.IsSome(FirstItem, await EnumerableWithMoreThanOneItem.SingleOrNoneAsync(item => item == FirstItem));
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
}
