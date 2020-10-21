using System.Linq;
using Funcky.Extensions;
using Xunit;

namespace Funcky.Test.Extensions.EnumerableExtensions
{
    public class WithIndexTest
    {
        [Fact]
        public void AnEmptySequenceWithIndexReturnsAnEmptySequence()
        {
            var emptySequence = Enumerable.Empty<string>();

            Assert.Empty(emptySequence.WithIndex());
        }

        [Fact]
        public void ASequenceWithOneElementWithIndexHasTheIndexZero()
        {
            const string expectedValue = "Hello world!";
            var oneElementSequence = Sequence.Return(expectedValue);

            foreach (var (value, index) in oneElementSequence.WithIndex())
            {
                Assert.Equal(expectedValue, value);
                Assert.Equal(0, index);
            }
        }

        [Fact]
        public void ASequenceWithMultipleElementsWithIndexHaveAscendingIndices()
        {
            var sequence = Enumerable.Range(0, 20);

            foreach (var valueWithIndex in sequence.WithIndex())
            {
                Assert.Equal(valueWithIndex.Value, valueWithIndex.Index);
            }
        }
    }
}
