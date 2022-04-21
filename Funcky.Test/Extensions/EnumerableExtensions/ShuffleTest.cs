using FsCheck;
using FsCheck.Xunit;
using Funcky.Test.TestUtils;
using Xunit.Sdk;

namespace Funcky.Test.Extensions.EnumerableExtensions;

public sealed class ShuffleTest
{
    [Fact]
    public void AShuffleIsEnumeratedEagerly()
    {
        var doNotEnumerate = new FailOnEnumerationSequence<object>();

        Assert.Throws<XunitException>(() => doNotEnumerate.Shuffle());
    }

    [Fact]
    public void AShuffleWithASpecificRandomDistributionAlwaysReturnsTheSameShuffle()
    {
        var source = Enumerable.Range(0, 16);

        Assert.Equal(Sequence.Return(3, 2, 6, 15, 14, 0, 5, 8, 11, 7, 9, 12, 1, 13, 10, 4), source.Shuffle(new System.Random(1337)));
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
