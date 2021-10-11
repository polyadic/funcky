using Funcky.Async.Extensions;
using Xunit;
using static Funcky.Async.Test.Extensions.AsyncEnumerableExtensions.TestData;

namespace Funcky.Async.Test.Extensions.AsyncEnumerableExtensions
{
    public sealed class PairwiseTest
    {
        [Fact]
        public async Task GivenAnEmptySequencePairwiseReturnsAnEmptySequence()
        {
            Assert.Empty(await EmptyEnumerable.Pairwise().ToListAsync());
        }

        [Fact]
        public async Task GivenASingleElementSequencePairwiseReturnsAnEmptySequence()
        {
            Assert.Empty(await EnumerableWithOneItem.Pairwise().ToListAsync());
        }

        [Fact]
        public async Task GivenATwoElementSequencePairwiseReturnsASequenceWithOneElementTheHeadAndTheTail()
        {
            var pairwise = await EnumerableTwoItems.Pairwise().ToListAsync();

            Assert.Single(pairwise, (42, 1337));
        }

        [Fact]
        public async Task GivenASequencePairWiseReturnsTheElementsPairwise()
        {
            const int numberOfElements = 20;
            var asyncSequence = Enumerable.Range(0, numberOfElements).ToAsyncEnumerable();

            var pairs = await asyncSequence.Pairwise().ToListAsync();
            Assert.Equal(numberOfElements - 1, pairs.Count());

            foreach (var (pair, index) in pairs.Select((pair, index) => (pair, index)))
            {
                Assert.Equal((index, index + 1), pair);
            }
        }
    }
}
