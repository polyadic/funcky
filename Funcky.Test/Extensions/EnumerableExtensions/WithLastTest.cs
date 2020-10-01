using System.Linq;
using Funcky.Extensions;
using Xunit;

namespace Funcky.Test.Extensions.EnumerableExtensions
{
    public class WithLastTest
    {
        [Fact]
        public void AnEmptySequenceWithLastReturnsAnEmptySequence()
        {
            var emptySequence = Enumerable.Empty<string>();

            Assert.Empty(emptySequence.WithLast());
        }

        [Fact]
        public void ASequenceWithOneElementWithFirstHasOneElementWhichIsMarkedLast()
        {
            const string expectedValue = "Hello world!";
            var oneElementSequence = Enumerable.Repeat(expectedValue, 1);

            foreach (var (value, isLast) in oneElementSequence.WithLast())
            {
                Assert.Equal(expectedValue, value);
                Assert.True(isLast);
            }
        }

        [Fact]
        public void ASequenceWithMultipleElementsWithLastMarksTheLastElement()
        {
            const int length = 20;
            var sequence = Enumerable.Range(1, length);

            foreach (var (value, isLast) in sequence.WithLast())
            {
                Assert.Equal(value == length, isLast);
            }
        }
    }
}
