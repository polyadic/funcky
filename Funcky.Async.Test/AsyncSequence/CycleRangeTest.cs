using FsCheck;
using FsCheck.Xunit;
using Funcky.Async.Test.TestUtilities;
using Funcky.Test.TestUtils;

namespace Funcky.Async.Test;

public sealed class CycleRangeTest
{
    [Fact]
    public async Task CycleRangeIsEnumeratedLazilyAsync()
    {
        var doNotEnumerate = new FailOnEnumerateAsyncSequence<object>();

        await using var cycleRange = AsyncSequence.CycleRange(doNotEnumerate);
    }

    [Fact]
    public void CyclingAnEmptySetThrowsAnArgumentException()
        => Assert.ThrowsAsync<InvalidOperationException>(CycleEmptySequenceAsync);

    [Property]
    public Property CycleRangeCanProduceArbitraryManyItemsAsync(NonEmptySet<int> sequence, PositiveInt arbitraryElements)
        => (GetArbitraryManyItemsAsync(sequence.Get, arbitraryElements.Get).Result == arbitraryElements.Get)
            .ToProperty();

    [Property]
    public Property CycleRangeRepeatsTheElementsArbitraryManyTimes(NonEmptySet<int> sequence, PositiveInt arbitraryElements)
        => CycleRangeRepeatsTheElementsArbitraryManyTimesAsync(sequence.Get.ToAsyncEnumerable(), arbitraryElements.Get)
            .Result.ToProperty();

    [Fact]
    public async Task CycleRangeEnumeratesUnderlyingEnumerableOnlyOnceAsync()
    {
        var sequence = Sequence.Return("Test", "Hello", "Do", "Wait");
        var enumerateOnce = AsyncEnumerateOnce.Create(sequence);

        await using var cycleRange = AsyncSequence.CycleRange(enumerateOnce);

        await cycleRange
            .Take(sequence.Count * 3)
            .ForEachAsync(NoOperation<string>);
    }

    private static async Task<int> GetArbitraryManyItemsAsync(IEnumerable<int> sequence, int arbitraryElements)
    {
        await using var cycleRange = AsyncSequence.CycleRange(sequence.ToAsyncEnumerable());

        return await cycleRange.Take(arbitraryElements).CountAsync();
    }

    private static async Task CycleEmptySequenceAsync()
    {
        await using var cycledRange = AsyncSequence.CycleRange(AsyncSequence.Return<string>());
        await using var enumerator = cycledRange.GetAsyncEnumerator();

        await enumerator.MoveNextAsync();
    }

    private async Task<bool> CycleRangeRepeatsTheElementsArbitraryManyTimesAsync(IAsyncEnumerable<int> asyncEnumerable, int arbitraryElements)
    {
        await using var cycleRange = AsyncSequence.CycleRange(asyncEnumerable);

        return await cycleRange
            .IsSequenceRepeating(asyncEnumerable)
            .NTimes(arbitraryElements);
    }
}
