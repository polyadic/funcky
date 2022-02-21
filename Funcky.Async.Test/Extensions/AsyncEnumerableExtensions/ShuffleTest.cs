using FsCheck;
using FsCheck.Xunit;
using Funcky.Async.Extensions;
using Funcky.Async.Test.TestUtilities;
using Xunit;
using Xunit.Sdk;

namespace Funcky.Async.Test.Extensions.AsyncEnumerableExtensions
{
    public sealed class ShuffleTest
    {
        [Fact]
        public void AShuffleIsEnumeratedLazilyAsync()
        {
            var doNotEnumerate = new FailOnEnumerateAsyncSequence<object>();

            var shuffled = doNotEnumerate.Shuffle();

            Assert.ThrowsAsync<XunitException>(async () => await shuffled);
        }

        [Fact]
        public async Task AShuffleWithASpecificRandomDistributionAlwaysReturnsTheSameShuffle()
        {
            var source = AsyncEnumerable.Range(0, 16);

            Assert.Equal(Sequence.Return(3, 2, 6, 15, 14, 0, 5, 8, 11, 7, 9, 12, 1, 13, 10, 4), await source.Shuffle(new System.Random(1337)));
        }

        [Property]
        public Property AShuffleHasTheSameElementsAsTheSource(List<int> source)
            => source
                .ToAsyncEnumerable()
                .Shuffle()
                .Result
                .All(source.Contains)
                .ToProperty();

        [Property]
        public Property AShuffleHasTheSameLengthAsTheSource(List<int> source)
            => (source.ToAsyncEnumerable().Shuffle().Result.Count() == source.Count)
                .ToProperty();
    }
}
