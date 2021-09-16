using FsCheck;
using FsCheck.Xunit;
using Funcky.Test.TestUtils;
using Xunit;

namespace Funcky.Test.Extensions.EnumerableExtensions
{
    public sealed class SlidingWindowTest
    {
        [Fact]
        public void ASlidingWindowIsEnumeratedLazily()
        {
            var doNotEnumerate = new FailOnEnumerateSequence<object>();

            _ = doNotEnumerate.SlidingWindow(42);
        }

        [Property]
        public void ASlidingWindowFromAnEmptySequenceIsAlwaysEmpty(PositiveInt width)
            => Enumerable
                .Empty<int>()
                .SlidingWindow(width.Get)
                .None()
                .ToProperty();

        [Fact]
        public void GivenASourceSequenceEqualInLengthToTheSlidingWindowWidthReturnsASequenceWithOneElement()
        {
            var source = Enumerable.Range(0, 5);

            Assert.Single(source.SlidingWindow(5));
            Assert.Equal(source, source.SlidingWindow(5).Single());
        }

        [Fact]
        public void GivenASourceSequenceShorterThanTheSlidingWindowWidthReturnsAnEmptySequence()
        {
            var source = Enumerable.Range(0, 5);

            Assert.Empty(source.SlidingWindow(10));
        }

        [Fact]
        public void SlidingWindowReturnsTheCorrectAmountOfWindowsAllOfEvenSize()
        {
            var width = 5;
            var source = Enumerable.Range(0, 10);
            var windows = source.SlidingWindow(width).ToList();

            Assert.Equal(6, windows.Count);
            windows.ForEach(w => Assert.Equal(width, w.Count()));
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(-42)]
        [InlineData(-1)]
        [InlineData(0)]
        public void SlidingWindowThrowsOnNonPositiveWidth(int width)
        {
            var source = Enumerable.Range(0, 10);

            Assert.Throws<ArgumentOutOfRangeException>(() => source.SlidingWindow(width));
        }

        [Fact]
        public void SlidingWindowReturnsASequenceOfConsecutiveWindows()
        {
            var width = 4;
            var source = Enumerable.Range(0, 6);

            Assert.Collection(
                source.SlidingWindow(width),
                window1 => { Assert.Equal(Enumerable.Range(0, width), window1); },
                window2 => { Assert.Equal(Enumerable.Range(1, width), window2); },
                window3 => { Assert.Equal(Enumerable.Range(2, width), window3); });
        }
    }
}
