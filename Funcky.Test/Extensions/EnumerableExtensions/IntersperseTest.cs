using Funcky.Test.TestUtils;
using Xunit;

namespace Funcky.Test.Extensions.EnumerableExtensions
{
    public sealed class IntersperseTest
    {
        [Fact]
        public void IntersperseIsEvaluatedLazily()
        {
            var doNotEnumerate = new FailOnEnumerateSequence<int>();
            _ = doNotEnumerate.Intersperse(42);
        }

        [Fact]
        public void InterspersingAnEmptyEnumerableReturnsAnEmptyEnumerable()
        {
            Assert.Empty(Enumerable.Empty<int>().Intersperse(42));
        }

        [Fact]
        public void InterspersingASequenceWithOneElementReturnsOriginalSequence()
        {
            var source = Sequence.Return(10);
            Assert.Equal(source, source.Intersperse(42));
        }

        [Theory]
        [InlineData(new[] { 1, 0, 2 }, new[] { 1, 2 })]
        [InlineData(new[] { 1, 0, 2, 0, 3 }, new[] { 1, 2, 3 })]
        [InlineData(new[] { 1, 0, 2, 0, 3, 0, 4 }, new[] { 1, 2, 3, 4 })]
        public void InterspersingASequenceWithMoreThanOneElementReturnsExpectedSequence(IEnumerable<int> expected, IEnumerable<int> source)
        {
            Assert.Equal(expected, source.Intersperse(0));
        }
    }
}
