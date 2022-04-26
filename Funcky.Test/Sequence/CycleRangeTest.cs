using System.Collections.Immutable;
using FsCheck;
using FsCheck.Xunit;
using Funcky.Test.TestUtils;

namespace Funcky.Test;

public sealed class CycleRangeTest
{
    [Fact]
    public void CycleRangeIsEnumeratedLazily()
    {
        var doNotEnumerate = new FailOnEnumerationSequence<object>();

        using var cycleRange = Sequence.CycleRange(doNotEnumerate);
    }

    [Fact]
    public void CyclingAnEmptySetThrowsAnArgumentException()
            => Assert.Throws<InvalidOperationException>(CycleEmptySequence);

    [Property]
    public Property CycleRangeCanProduceArbitraryManyItems(NonEmptySet<int> sequence, PositiveInt arbitraryElements)
    {
        using var cycleRange = Sequence
            .CycleRange(sequence.Get);

        return (cycleRange.Take(arbitraryElements.Get).Count() == arbitraryElements.Get)
            .ToProperty();
    }

    [Property]
    public Property CycleRangeRepeatsTheElementsArbitraryManyTimes(NonEmptySet<int> sequence, PositiveInt arbitraryElements)
    {
        using var cycleRange = Sequence
            .CycleRange(sequence.Get);

        return cycleRange
            .IsSequenceRepeating(sequence.Get)
            .NTimes(arbitraryElements.Get);
    }

    [Property]
    public void CycleRangeEnumeratesUnderlyingEnumerableOnlyOnce(NonEmptySet<int> sequence)
    {
        var enumerateOnce = new EnumerateOnce<int>(sequence.Get);

        using var cycleRange = Sequence
            .CycleRange(enumerateOnce);

        cycleRange
            .Take(sequence.Get.Count * 3)
            .ForEach(NoOperation);
    }

    private static void CycleEmptySequence()
    {
        using var cycledRange = Sequence.CycleRange(ImmutableList<string>.Empty);
        using var enumerator = cycledRange.GetEnumerator();

        enumerator.MoveNext();
    }
}
