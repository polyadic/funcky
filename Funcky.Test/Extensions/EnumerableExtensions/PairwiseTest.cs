using Funcky.Test.TestUtils;

namespace Funcky.Test.Extensions.EnumerableExtensions
{
    public sealed class PairwiseTest
    {
        [Fact]
        public void PairwiseIsEnumeratedLazily()
        {
            var doNotEnumerate = new FailOnEnumerationSequence<object>();

            _ = doNotEnumerate.Pairwise();
        }

        [Fact]
        public void GivenAnEmptySequencePairwiseReturnsAnEmptySequence()
        {
            var emptySequence = Enumerable.Empty<string>();

            Assert.Empty(emptySequence.Pairwise());
        }

        [Fact]
        public void GivenASingleElementSequencePairwiseReturnsAnEmptySequence()
        {
            var oneElementSequence = Sequence.Return("string");

            Assert.Empty(oneElementSequence.Pairwise());
        }

        [Fact]
        public void GivenATwoElementSequencePairwiseReturnsASequenceWithOneElementTheHeadAndTheTail()
        {
            var twoElementSequence = new List<int> { 42, 1337 };

            Assert.Single(twoElementSequence.Pairwise(), (42, 1337));
        }

        [Fact]
        public void GivenASequencePairWiseReturnsTheElementsPairwise()
        {
            const int numberOfElements = 20;
            var sequence = Enumerable.Range(0, numberOfElements);

            Assert.Equal(numberOfElements - 1, sequence.Pairwise().Count());

            foreach (var (pair, index) in sequence.Pairwise().Select((pair, index) => (pair, index)))
            {
                Assert.Equal((index, index + 1), pair);
            }
        }
    }
}
