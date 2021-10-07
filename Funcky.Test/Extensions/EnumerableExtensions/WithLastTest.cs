using Funcky.Test.TestUtils;

namespace Funcky.Test.Extensions.EnumerableExtensions
{
    public sealed class WithLastTest
    {
        [Fact]
        public void WithLastIsEnumeratedLazily()
        {
            var doNotEnumerate = new FailOnEnumerationSequence<object>();

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

        [Fact]
        public void ElementAtAccessIsOptimizedOnAnIListSourceWithIndex()
        {
            var length = 5000;
            var nonEnumerableList = new FailOnEnumerationList(length);
            var listWithLast = nonEnumerableList.WithLast();

            Assert.Equal(1337, listWithLast.ElementAt(1337).Value);

            Assert.False(listWithLast.ElementAt(0).IsLast);
            Assert.False(listWithLast.ElementAt(1).IsLast);
            Assert.False(listWithLast.ElementAt(2500).IsLast);
            Assert.False(listWithLast.ElementAt(length - 2).IsLast);
            Assert.True(listWithLast.ElementAt(length - 1).IsLast);
        }

        [Fact]
        public void OptimizedSourceWithIndexCanBeEnumerated()
        {
            var length = 222;
            var nonEnumerableList = Enumerable.Range(0, length).ToList();

            Assert.Equal(length, nonEnumerableList.WithLast().Aggregate(0, (sum, _) => sum + 1));
        }
    }
}
