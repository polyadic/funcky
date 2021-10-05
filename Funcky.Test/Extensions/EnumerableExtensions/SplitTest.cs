using FsCheck;
using FsCheck.Xunit;
using Funcky.Test.TestUtils;

namespace Funcky.Test.Extensions.EnumerableExtensions
{
    public sealed class SplitTest
    {
        [Fact]
        public void SplitIsEnumeratedLazily()
        {
            var doNotEnumerate = new FailOnEnumerationSequence<object>();

            _ = doNotEnumerate.Split(42);
        }

        [Property]
        public Property SplittingAnEmptyEnumerableAlwaysReturnsAnEmptyEnumerable(int separator)
        {
            var parts = Enumerable.Empty<int>().Split(separator);

            return (!parts.Any()).ToProperty();
        }

        [Fact]
        public void SplitAnEnumerableCorrectly()
        {
            IEnumerable<int> sequence = new List<int> { 12, 14, 7, 41, 31, 19, 7, 9, 11, 99, 99 };

            var parts = sequence.Split(7);

            Assert.Collection(
                parts,
                part => Assert.Equal(new List<int> { 12, 14 }, part),
                part => Assert.Equal(new[] { 41, 31, 19 }, part),
                part => Assert.Equal(new[] { 9, 11, 99, 99 }, part));
        }
    }
}
