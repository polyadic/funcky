using System.Linq;
using Funcky.Extensions;
using Xunit;

namespace Funcky.Test.Extensions.EnumerableExtensions
{
    public class WithFirstTest
    {
        [Fact]
        public void AnEmptySequenceWithFirstReturnsAnEmptySequence()
        {
            var emptySequence = Enumerable.Empty<string>();

            Assert.Empty(emptySequence.WithFirst());
        }

        [Fact]
        public void ASequenceWithOneElementWithFirstHasOneElementWhichIsMarkedFirst()
        {
            const string expectedValue = "Hello world!";
            var oneElementSequence = Enumerable.Repeat(expectedValue, 1);

            foreach (var (value, isFirst) in oneElementSequence.WithFirst())
            {
                Assert.Equal(expectedValue, value);
                Assert.True(isFirst);
            }
        }

        [Fact]
        public void ASequenceWithMultipleElementsWithFirstMarksTheFirstElement()
        {
            const int length = 20;

            var sequence = Enumerable.Range(1, length);

            foreach (var valueWithFirst in sequence.WithFirst())
            {
                Assert.Equal(valueWithFirst.Value == 1, valueWithFirst.IsFirst);
            }
        }
    }
}
