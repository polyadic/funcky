using Funcky.Async.Test.TestUtilities;
using Funcky.Monads;
using Xunit;

namespace Funcky.Async.Test
{
    public sealed class SuccessorsTest
    {
        [Fact]
        public async Task ReturnsEmptySequenceWhenFirstItemIsNoneAsync()
        {
            await AsyncAssert.Empty(AsyncSequence.Successors(Option<int>.None, AsyncIdentity));
        }

        [Fact]
        public async Task ReturnsOnlyTheFirstItemWhenSuccessorFunctionImmediatelyReturnsNoneAsync()
        {
            var first = AsyncAssert.Single(AsyncSequence.Successors(10, _ => ValueTask.FromResult(Option<int>.None)));
            Assert.Equal(10, await first);
        }

        [Fact]
        public async Task SuccessorsWithNonOptionFunctionReturnsEndlessEnumerable()
        {
            const int count = 40;
            Assert.Equal(count, await AsyncSequence.Successors(0, AsyncIdentity).Take(count).CountAsync());
        }

        [Fact]
        public async Task SuccessorsReturnsEnumerableThatReturnsValuesBasedOnSeed()
        {
            await AsyncAssert.Equal(
                AsyncEnumerable.Range(0, 10),
                AsyncSequence.Successors(0, i => ValueTask.FromResult(i + 1)).Take(10));
        }

        [Fact]
        public async Task SuccessorsReturnsEnumerableThatReturnsItemUntilNoneIsReturnedFromFunc()
        {
            await AsyncAssert.Equal(
                AsyncEnumerable.Range(0, 11),
                AsyncSequence.Successors(0, i => ValueTask.FromResult(Option.FromBoolean(i < 10, i + 1))));
        }

        private static ValueTask<Option<TItem>> AsyncIdentity<TItem>(TItem item)
            where TItem : notnull
            => ValueTask.FromResult<Option<TItem>>(item);
    }
}
