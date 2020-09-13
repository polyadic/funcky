using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Funcky.Extensions;
using Funcky.Xunit;
using Xunit;
using static Funcky.Functional;

namespace Funcky.Test.Extensions
{
    public sealed class AsyncEnumerableExtensionsTest
    {
        private const string FirstItem = "first";

        private const string LastItem = "last";

        private static readonly IAsyncEnumerable<string> EmptyEnumerable
            = AsyncEnumerable.Empty<string>();

        private static readonly IAsyncEnumerable<string> EnumerableWithOneItem
            = AsyncEnumerable.Repeat(FirstItem, 1);

        private static readonly IAsyncEnumerable<string> EnumerableWithMoreThanOneItem
            = new[] { FirstItem, "foo", LastItem }.ToAsyncEnumerable();

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
