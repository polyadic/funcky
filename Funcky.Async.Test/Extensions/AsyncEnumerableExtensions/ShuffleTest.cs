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
