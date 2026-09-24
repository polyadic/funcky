#pragma warning disable SA1010 // StyleCop support for collection expressions is missing
using Funcky.Test.TestUtilities;

namespace Funcky.Test.Extensions.EnumerableExtensions;

public sealed class InspectEmptyTest
{
    [Fact]
    public void InspectEmptyIsEnumeratedLazily()
    {
        var doNotEnumerate = new FailOnEnumerationSequence<object>();
        _ = doNotEnumerate.InspectEmpty(NoOperation);
    }

    [Fact]
    public void InspectEmptyExecutesAnInspectionFunctionOnMaterializationOnAnEmptyEnumerable()
    {
        var sideEffect = 0;
        IEnumerable<string> enumerable = [];

        _ = enumerable.InspectEmpty(() => sideEffect = 1).Materialize();

        Assert.Equal(1, sideEffect);
    }

    [Fact]
    public void InspectEmptyExecutesNoInspectionFunctionOnMaterializationOnANonEmptyEnumerable()
    {
        var sideEffect = 0;
        IEnumerable<string> enumerable = ["Hello", "World"];

        _ = enumerable.InspectEmpty(() => sideEffect = 1).Materialize();

        Assert.Equal(0, sideEffect);
    }
}
