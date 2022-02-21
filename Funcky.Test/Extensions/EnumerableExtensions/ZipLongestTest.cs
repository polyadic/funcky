using Funcky.Test.TestUtils;

namespace Funcky.Test.Extensions.EnumerableExtensions
{
    public sealed class ZipLongestTest
    {
        [Fact]
        public void ZipLongestIsEnumeratedLazily()
        {
            var doNotEnumerate = new FailOnEnumerationSequence<object>();

            _ = doNotEnumerate.ZipLongest(doNotEnumerate, _ => Unit.Value);
        }

        [Fact]
        public void GivenTwoEmptySequencesZipLongestReturnsAnEmptySequence()
        {
            var numbers = Enumerable.Empty<int>();
            var strings = Enumerable.Empty<string>();

            var zipped = numbers.ZipLongest(strings, _ => Unit.Value);

            Assert.Empty(zipped);
        }

        [Fact]
        public void GivenTwoSequencesOfTheSameLengthWeGetNoNoneValue()
        {
            var numbers = Enumerable.Range(0, 3);
            var strings = new[] { "Alpha", "Beta", "Gamma" };

            var zipped = numbers
                .ZipLongest(strings)
                .ToList();

            Assert.Equal(3, zipped.Count);
            foreach (var value in zipped)
            {
                Assert.True(value.Match(left: False, right: False, both: True));
            }
        }

        [Fact]
        public void GivenTwoSequencesWeOfDifferentLengthWeGetTheLongerAndFillWithNone()
        {
            var numbers = Enumerable.Range(0, 10);
            var strings = new[] { "Alpha", "Beta", "Gamma" };

            var zipped = numbers
                .ZipLongest(strings)
                .ToList();

            Assert.Equal(10, zipped.Count);

            Assert.True(zipped.First().Match(
                left: False,
                right: False,
                both: (left, right) => left == 0 && right == "Alpha"));

            Assert.True(zipped.Last().Match(
                left: left => left == 9,
                right: False,
                both: False));
        }
    }
}
