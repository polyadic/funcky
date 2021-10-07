using FsCheck;
using FsCheck.Xunit;
using Funcky.Test.TestUtils;

namespace Funcky.Test.Extensions.EnumerableExtensions
{
    public sealed class TakeEveryTest
    {
        [Fact]
        public void TakeEveryIsEnumeratedLazily()
        {
            var doNotEnumerate = new FailOnEnumerationSequence<object>();

            _ = doNotEnumerate.TakeEvery(42);
        }

        [Property]
        public Property TakeEveryOnAnEmptySequenceReturnsAnEmptySequence(PositiveInt interval)
            => Enumerable
                .Empty<string>()
                .TakeEvery(interval.Get)
                .None()
                .ToProperty();

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
            var numbers = Enumerable.Range(-60, 120).ToList();

            Assert.Equal(numbers.Count / 6, numbers.TakeEvery(6).Count());

            numbers.TakeEvery(6).ForEach(n => Assert.True(n % 6 == 0));
        }

        [Fact]
        public void TakeEveryWithLessThanIntervalElementsReturnsOnlyFirstElement()
        {
            var numbers = new List<int> { 1, 2, 3, 4 };

            Assert.Single(numbers.TakeEvery(4), expected: 1);
        }

        [Fact]
        public void TakeEveryWithASourceWith5ElementsAndInterval4Returns2Elements()
        {
            var numbers = new List<int> { 1, 2, 3, 4, 5 };

            Assert.Equal(new[] { 1, 5 }, numbers.TakeEvery(4));
        }
    }
}
