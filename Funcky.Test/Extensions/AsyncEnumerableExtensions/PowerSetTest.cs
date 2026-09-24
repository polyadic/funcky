#if INTEGRATED_ASYNC
using Funcky.Async.Test.TestUtilities;

namespace Funcky.Test.Extensions.AsyncEnumerableExtensions;

public sealed class PowerSetTest
{
    [Fact]
    public void APowerSetIsEnumeratedLazily()
    {
        var doNotEnumerate = new FailOnEnumerateAsyncSequence<object>();

        _ = doNotEnumerate.PowerSet();
    }

    [Fact]
    public async Task ThePowerSetOfTheEmptySetIsSetOfTheEmptySet()
    {
        var powerSet = AsyncEnumerable.Empty<string>().PowerSet();

        Assert.Empty(await powerSet.FirstAsync());
    }

    [Fact]
    public async Task ThePowerSetIsTheSetOfAllSubSets()
    {
        var sequence = AsyncSequence.Return("Alpha", "Beta", "Gamma");
        var powerSet = sequence.PowerSet();

        await AsyncAssert.Collection(
            powerSet,
            subset => { Assert.Equal(Enumerable.Empty<string>(), subset); },
            subset => { Assert.Equal(Sequence.Return("Alpha"), subset); },
            subset => { Assert.Equal(Sequence.Return("Beta"), subset); },
            subset => { Assert.Equal(Sequence.Return("Alpha", "Beta"), subset); },
            subset => { Assert.Equal(Sequence.Return("Gamma"), subset); },
            subset => { Assert.Equal(Sequence.Return("Alpha", "Gamma"), subset); },
            subset => { Assert.Equal(Sequence.Return("Beta", "Gamma"), subset); },
            subset => { Assert.Equal(Sequence.Return("Alpha", "Beta", "Gamma"), subset); });
    }
}
#endif
