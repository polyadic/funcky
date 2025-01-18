using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using Funcky.Test.TestUtils;

namespace Funcky.Test;

public sealed class CycleMaterializedTest
{
    [Fact]
    public void IsEnumeratedLazily()
    {
        var doNotEnumerate = new FailOnEnumerateReadOnlyCollection<object>(Count: 1);
        _ = Sequence.CycleMaterialized(doNotEnumerate);
    }

    [Fact]
    public void CyclingAnEmptySetThrowsAnException()
        => Assert.Throws<InvalidOperationException>(CycleEmptySequence);

    [Property]
    public Property CanProduceArbitraryManyItems(NonEmptySet<int> sequence, PositiveInt arbitraryElements)
    {
        var cycleRange = Sequence.CycleMaterialized(sequence.Get.Materialize());

        return (cycleRange.Take(arbitraryElements.Get).Count() == arbitraryElements.Get)
            .ToProperty();
    }

    [Property]
    public Property RepeatsTheElementsArbitraryManyTimes(NonEmptySet<int> sequence, PositiveInt arbitraryElements)
    {
        var cycleRange = Sequence.CycleMaterialized(sequence.Get.Materialize());

        return cycleRange
            .IsSequenceRepeating(sequence.Get)
            .NTimes(arbitraryElements.Get)
            .ToProperty();
    }

    private static void CycleEmptySequence()
    {
        var cycledRange = Sequence.CycleMaterialized(Array.Empty<int>());
        using var enumerator = cycledRange.GetEnumerator();

        enumerator.MoveNext();
    }
}
