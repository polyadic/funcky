using System;
using System.Linq;
using System.Threading.Tasks;
using Funcky.Async.Extensions;
using Xunit;
using static Funcky.Async.Test.Extensions.AsyncEnumerableExtensions.TestData;

namespace Funcky.Async.Test.Extensions.AsyncEnumerableExtensions
{
    public sealed class TakeEveryTest
    {
        [Fact]
        public async Task TakeEveryOnAnEmptySequenceReturnsAnEmptySequence()
        {
            Assert.Empty(await EmptyEnumerable.TakeEvery(5).ToListAsync());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-42)]
        public async Task TakeEveryOnlyAcceptsPositiveIntervals(int negativeIntervals)
        {
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await EnumerableWithOneItem.TakeEvery(negativeIntervals).ToListAsync());
        }

        [Fact]
        public async Task TakeEverySelectsEveryNthElement()
        {
            var numbers = AsyncEnumerable.Range(-60, 120);

            Assert.Equal(await numbers.CountAsync() / 6, await numbers.TakeEvery(6).CountAsync());

            await numbers.TakeEvery(6).ForEachAsync(n => Assert.True(n % 6 == 0));
        }

        [Fact]
        public async Task TakeEveryWithLessThanIntervalElementsReturnsOnlyFirstElement()
        {
            var everyThird = await EnumerableWithMoreThanOneItem.TakeEvery(3).ToListAsync();

            Assert.Single(everyThird);
            Assert.Equal("first", everyThird.Single());
        }

        [Fact]
        public async Task TakeEveryWithASourceWith5ElementsAndInterval4Returns2Elements()
        {
            Assert.Equal(new[] { 1, 5 }, await OneToFive.TakeEvery(4).ToListAsync());
        }
    }
}
