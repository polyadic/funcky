using Funcky.Test.TestUtilities;

namespace Funcky.Test.Extensions.EnumerableExtensions;

public sealed class InspectTest
{
    [Fact]
    public void InspectIsEnumeratedLazily()
    {
        var doNotEnumerate = new FailOnEnumerationSequence<object>();
        _ = doNotEnumerate.Inspect(NoOperation);
    }

#if NET6_0_OR_GREATER
    [Fact]
    public void InspectPreservesNonEnumeratedItemCount()
    {
        var source = Sequence.Return(1, 2, 3);
        Assert.True(source.TryGetNonEnumeratedCount(out var expectedCount));

        var inspected = source.Inspect(NoOperation);

        Assert.True(inspected.TryGetNonEnumeratedCount(out var count));
        Assert.Equal(expectedCount, count);
    }
#endif

    [Fact]
    public void GivenAnEnumerableAndInjectWeCanApplySideEffectsToEnumerables()
    {
        var sideEffect = 0;
        var numbers = Sequence.Return(1, 2, 3, 42);

        var numbersWithSideEffect = numbers
            .Inspect(_ => { ++sideEffect; });

        Assert.Equal(0, sideEffect);

        numbersWithSideEffect.ForEach(NoOperation);

        Assert.Equal(numbers.Count, sideEffect);
    }
}
