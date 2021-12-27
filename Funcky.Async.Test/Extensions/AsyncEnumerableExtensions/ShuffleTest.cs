using FsCheck;
using FsCheck.Xunit;
using Funcky.Async.Extensions;
using Funcky.Async.Test.TestUtilities;
using Xunit;

namespace Funcky.Async.Test.Extensions.AsyncEnumerableExtensions
{
    public sealed class ShuffleTest
    {
        [Fact]
        public void AShuffleIsEnumeratedLazily()
        {
            var doNotEnumerate = new FailOnEnumerateAsyncSequence<object>();

            _ = doNotEnumerate.Shuffle();
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
