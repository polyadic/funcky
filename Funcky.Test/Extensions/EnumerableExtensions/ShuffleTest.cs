using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
#if !SHUFFLE_EXTENSION
using Funcky.Test.TestUtilities;
using Xunit.Sdk;
#endif

namespace Funcky.Test.Extensions.EnumerableExtensions;

public sealed class ShuffleTest
{
#if !SHUFFLE_EXTENSION
    [Fact]
    public void AShuffleIsEnumeratedEagerly()
    {
        var doNotEnumerate = new FailOnEnumerationSequence<object>();

        Assert.Throws<XunitException>(() => doNotEnumerate.Shuffle());
    }
#endif

    [Fact]
    public void AShuffleWithASpecificRandomDistributionAlwaysReturnsTheSameShuffle()
    {
        var source = Enumerable.Range(0, 16);

        Assert.Equal(Enumerable.Return(3, 2, 6, 15, 14, 0, 5, 8, 11, 7, 9, 12, 1, 13, 10, 4), source.Shuffle(new Random(1337)));
    }

    [Property]
    public Property AShuffleIsAPermutationOfTheSource(List<int> source, int seed)
        => source
            .Shuffle(new Random(seed))
            .OrderBy(Identity)
            .SequenceEqual(source.OrderBy(Identity))
            .ToProperty();

    [Property]
    public Property AShuffleWithTheSameSeedIsReproducible(List<int> source, int seed)
        => source
            .Shuffle(new Random(seed))
            .SequenceEqual(source.Shuffle(new Random(seed)))
            .ToProperty();

#if !SHUFFLE_EXTENSION
    // Two shuffles of 32 elements agree with probability 1/32!, unless the seedless overload seeds from the clock
    // and both calls land in the same tick.
    [Fact]
    public void TwoConsecutiveSeedlessShufflesDiffer()
    {
        var source = Enumerable.Range(0, 32).ToList();
        _ = source.Shuffle(); // warm up, so that JIT time does not push the two calls into different ticks

        Assert.NotEqual(source.Shuffle(), source.Shuffle());
    }
#endif

    [Property]
    public Property AShuffleHasTheSameLengthAsTheSource(List<int> source)
        => (source.Shuffle().Count() == source.Count)
            .ToProperty();
}
