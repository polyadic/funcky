using System.Collections.Generic;
using System.Linq;
using Funcky.Extensions;
using Xunit;

namespace Funcky.Test.Extensions
{
    public sealed class PairwiseTest
    {
        [Fact]
        public void GivenAnEmptySequencePairwiseReturnsAnEmptySequence()
        {
            var emptySequence = Enumerable.Empty<string>();

            Assert.Empty(emptySequence.Pairwise());
        }

        [Fact]
        public void GivenASingleElementSequencePairwiseReturnsAnEmptySequence()
        {
            var oneElementSequence = Enumerable.Repeat("string", 1);

            Assert.Empty(oneElementSequence.Pairwise());
        }

        [Fact]
        public void GivenATwoElementSequencePairwiseReturnsASequenceWithOneElementTheHeadAndTheTail()
        {
            var twoElementSequence = new List<int> { 42, 1337 };

            Assert.Single(twoElementSequence.Pairwise());

            var (head, tail) = twoElementSequence.Pairwise().First();
            Assert.Equal(42, head);
            Assert.Equal(1337, tail);
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
