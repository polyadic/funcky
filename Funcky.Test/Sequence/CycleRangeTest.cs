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
    public void CyclingAnEmptySetThrowsAnException()
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
            .NTimes(arbitraryElements.Get)
            .ToProperty();
    }

    [Fact]
    public void CycleRangeEnumeratesUnderlyingEnumerableOnlyOnce()
    {
        var sequence = Sequence.Return("Test", "Hello", "Do", "Wait");
        var enumerateOnce = EnumerateOnce.Create(sequence);

        using var cycleRange = Sequence
            .CycleRange(enumerateOnce);

        cycleRange
            .Take(sequence.Count * 3)
            .ForEach(NoOperation);
    }

    private static void CycleEmptySequence()
    {
        using var cycledRange = Sequence.CycleRange(Sequence.Return<string>());
        using var enumerator = cycledRange.GetEnumerator();

        enumerator.MoveNext();
    }
}
