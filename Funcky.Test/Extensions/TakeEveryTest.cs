using System;
using System.Collections.Generic;
using System.Linq;
using Funcky.Extensions;
using Xunit;

namespace Funcky.Test.Extensions.AsyncEnumerableExtensions
{
    public sealed class TakeEveryTest
    {
        [Fact]
        public void TakeEveryOnAnEmptySequnceReturnsAnEmptySequence()
        {
            var emptySource = Enumerable.Empty<string>();

            Assert.Empty(emptySource.TakeEvery(5));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-42)]
        public void TakeEveryOnlyAcceptsPositiveIntervals(int negativeIntervals)
        {
            var numbers = new List<int> { 1 };

            Assert.Throws<ArgumentOutOfRangeException>(() => numbers.TakeEvery(negativeIntervals));
        }

        [Fact]
        public void TakeEverySelectsEveryNthElement()
        {
            var numbers = Enumerable.Range(-60, 120);

            Assert.Equal(numbers.Count() / 6, numbers.TakeEvery(6).Count());

            numbers.TakeEvery(6).ForEach(n => Assert.True(n % 6 == 0));
        }

        [Fact]
        public void TakeEveryWithLessThanIntervalElementsReturnsOnlyFirstElement()
        {
            var numbers = new List<int> { 1, 2, 3, 4 };

            Assert.Collection(numbers.TakeEvery(4), element => Assert.Equal(1, element));
        }

        [Fact]
        public void TakeEveryWithASourceWith5ElementsAndInterval4Returns2Elements()
        {
            var numbers = new List<int> { 1, 2, 3, 4, 5 };

            Assert.Collection(numbers.TakeEvery(4), element => Assert.Equal(1, element), element => Assert.Equal(5, element));
        }
    }
}
