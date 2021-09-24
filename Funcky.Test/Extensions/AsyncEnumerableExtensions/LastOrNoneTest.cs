using static Funcky.Test.Extensions.AsyncEnumerableExtensions.TestData;

namespace Funcky.Test.Extensions.AsyncEnumerableExtensions
{
    public sealed class LastOrNoneTest
    {
        [Fact]
        public async Task LastOrNoneReturnsNoneWhenEnumerableIsEmpty()
        {
            FunctionalAssert.IsNone(await EmptyEnumerable.LastOrNoneAsync());
        }

        [Fact]
        public async Task LastOrNoneReturnsItemWhenEnumerableHasOneItem()
        {
            FunctionalAssert.IsSome(
                FirstItem,
                await EnumerableWithOneItem.LastOrNoneAsync());
        }

        [Fact]
        public async Task LastOrNoneReturnsNoneWhenEnumerableHasOneItemButItDoesNotMatchPredicate()
        {
            FunctionalAssert.IsNone(
                await EnumerableWithOneItem.LastOrNoneAsync(False));
        }

        [Fact]
        public async Task LastOrNoneReturnsLastItemWhenEnumerableHasMoreThanOneItem()
        {
            FunctionalAssert.IsSome(
                LastItem,
                await EnumerableWithMoreThanOneItem.LastOrNoneAsync());
        }

        [Fact]
        public async Task LastOrNoneReturnsNoneWhenEnumerableHasItemsButNoneMatchesPredicate()
        {
            FunctionalAssert.IsNone(await EnumerableWithMoreThanOneItem.LastOrNoneAsync(False));
        }
    }
}
