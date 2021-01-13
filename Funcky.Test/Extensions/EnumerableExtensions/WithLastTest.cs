using System.Linq;
using Funcky.Extensions;
using Funcky.Test.TestUtils;
using Xunit;

namespace Funcky.Test.Extensions.EnumerableExtensions
{
    public sealed class WithLastTest
    {
        [Fact]
        public void WithLastIsEnumeratedLazily()
        {
            var doNotEnumerate = new FailOnEnumerateSequence<object>();

            _ = doNotEnumerate.WithLast();
        }

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
            var oneElementSequence = Sequence.Return(expectedValue);

            var sequenceWithLast = oneElementSequence.WithLast();
            foreach (var (value, isLast) in sequenceWithLast)
            {
                Assert.Equal(expectedValue, value);
                Assert.True(isLast);
            }

            Assert.NotEmpty(sequenceWithLast);
        }

        [Fact]
        public void ASequenceWithMultipleElementsWithLastMarksTheLastElement()
        {
            const int length = 20;
            var sequence = Enumerable.Range(1, length);

            foreach (var valueWithLast in sequence.WithLast())
            {
                Assert.Equal(valueWithLast.Value == length, valueWithLast.IsLast);
            }
        }
    }
}
