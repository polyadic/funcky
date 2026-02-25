using Funcky.Test.TestUtilities;

namespace Funcky.Test.Extensions.EnumerableExtensions;

public sealed class PowerSetTest
{
    [Fact]
    public void APowerSetIsEnumeratedLazily()
    {
        var doNotEnumerate = new FailOnEnumerationSequence<object>();

        _ = doNotEnumerate.PowerSet();
    }

    [Fact]
    public void ThePowerSetOfTheEmptySetIsSetOfTheEmptySet()
    {
        var powerSet = Enumerable.Empty<string>().PowerSet();

        Assert.Empty(powerSet.First());
    }

    [Fact]
    public void ThePowerSetIsTheSetOfAllSubSets()
    {
        var sequence = Sequence.Return("Alpha", "Beta", "Gamma");
        var powerSet = sequence.PowerSet();

        Assert.Collection(
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
