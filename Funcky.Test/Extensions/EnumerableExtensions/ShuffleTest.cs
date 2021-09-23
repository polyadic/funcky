using FsCheck;
using FsCheck.Xunit;
using Funcky.Test.TestUtils;
using Xunit;

namespace Funcky.Test.Extensions.EnumerableExtensions
{
    public sealed class ShuffleTest
    {
        [Fact]
        public void AShuffleIsEnumeratedLazily()
        {
            var doNotEnumerate = new FailOnEnumerationSequence<object>();

            _ = doNotEnumerate.Shuffle();
        }

        [Property]
        public Property AShuffleHasTheSameElementsAsTheSource(List<int> source)
            => source
                .Shuffle()
                .All(source.Contains)
                .ToProperty();

        [Property]
        public Property AShuffleHasTheSameLengthAsTheSource(List<int> source)
            => (source.Shuffle().Count() == source.Count)
                .ToProperty();
    }
}
