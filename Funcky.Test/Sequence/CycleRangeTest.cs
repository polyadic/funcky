#pragma warning disable λ1202 // This test exercises the classic Sequence form itself.

using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using Funcky.Test.TestUtilities;

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

    [Fact]
    public void CycleRangeAcquiresTheSourceEnumeratorOnlyWhenTheFirstElementIsRequested()
    {
        var source = new DisposeTrackingSequence<int>([1, 2, 3]);
        using var cycleRange = Sequence.CycleRange(source);

        using var enumerator = cycleRange.GetEnumerator();
        Assert.Equal(0, source.EnumeratorsCreated);

        Assert.True(enumerator.MoveNext());
        Assert.Equal(1, source.EnumeratorsCreated);
    }

    [Fact]
    public void CycleRangeDisposesTheSourceEnumeratorOnceTheSourceIsExhausted()
    {
        var source = new DisposeTrackingSequence<int>([1, 2, 3]);
        using var cycleRange = Sequence.CycleRange(source);

        Assert.Equal([1, 2, 3, 1], cycleRange.Take(4));
        Assert.Equal(1, source.EnumeratorsDisposed);

        Assert.Equal([1, 2, 3, 1, 2], cycleRange.Take(5));
        Assert.Equal(1, source.EnumeratorsCreated);
    }

    private static void CycleEmptySequence()
    {
        using var cycledRange = Sequence.CycleRange(Sequence.Return<string>());
        using var enumerator = cycledRange.GetEnumerator();

        enumerator.MoveNext();
    }
}
