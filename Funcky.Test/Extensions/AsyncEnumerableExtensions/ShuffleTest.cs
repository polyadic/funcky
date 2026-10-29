#if ASYNC_SUPPORTED
using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using Funcky.Async.Test.TestUtilities;
using Xunit.Sdk;

namespace Funcky.Async.Test.Extensions.AsyncEnumerableExtensions;

public sealed class ShuffleTest
{
    [Fact]
    public async Task AShuffleIsEnumeratedLazilyAsync()
    {
        var doNotEnumerate = new FailOnEnumerateAsyncSequence<object>();

        var shuffled = doNotEnumerate.ShuffleAsync();

        await Assert.ThrowsAsync<XunitException>(async () => await shuffled);
    }

    [Fact]
    public async Task AShuffleWithASpecificRandomDistributionAlwaysReturnsTheSameShuffle()
    {
        var source = AsyncEnumerable.Range(0, 16);

        Assert.Equal(Enumerable.Return(3, 2, 6, 15, 14, 0, 5, 8, 11, 7, 9, 12, 1, 13, 10, 4), await source.ShuffleAsync(new System.Random(1337)));
    }

    [Property]
    public Property AShuffleIsAPermutationOfTheSource(List<int> source, int seed)
        => source
            .ToAsyncEnumerable()
            .ShuffleAsync(new System.Random(seed))
            .Result
            .OrderBy(Identity)
            .SequenceEqual(source.OrderBy(Identity))
            .ToProperty();

    // Two shuffles of 32 elements agree with probability 1/32!, unless the seedless overload seeds from the clock
    // and both calls land in the same tick.
    [Fact]
    public async Task TwoConsecutiveSeedlessShufflesDiffer()
    {
        var source = Enumerable.Range(0, 32).ToList();
        _ = await source.ToAsyncEnumerable().ShuffleAsync(); // warm up, so that JIT time does not push the two calls into different ticks

        Assert.NotEqual(await source.ToAsyncEnumerable().ShuffleAsync(), await source.ToAsyncEnumerable().ShuffleAsync());
    }

    [Property]
    public Property AShuffleHasTheSameLengthAsTheSource(List<int> source)
        => (source.ToAsyncEnumerable().ShuffleAsync().Result.Count == source.Count)
            .ToProperty();
}
#endif
