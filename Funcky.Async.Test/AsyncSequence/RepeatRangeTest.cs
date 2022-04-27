using FsCheck;
using FsCheck.Xunit;
using Funcky.Async.Test.TestUtilities;
using Funcky.Test.TestUtils;

namespace Funcky.Async.Test;

public sealed class RepeatRangeTest
{
    [Fact]
    public async Task RepeatRangeIsEnumeratedLazily()
    {
        var doNotEnumerate = new FailOnEnumerateAsyncSequence<object>();

        await using var repeatRange = AsyncSequence.RepeatRange(doNotEnumerate, 2);
    }

    [Fact]
    public async Task RepeatRangeThrowsWhenAlreadyDisposedAsync()
    {
        var repeatRange = AsyncSequence.RepeatRange(AsyncSequence.Return(1337), 5);

#pragma warning disable IDISP016 // we test behaviour after Dispose
#pragma warning disable IDISP017 // we test behaviour after Dispose
        await repeatRange.DisposeAsync();
#pragma warning restore IDISP016
#pragma warning restore IDISP017

        await Assert.ThrowsAsync<ObjectDisposedException>(() => repeatRange.ForEachAsync(NoOperation<int>));
    }

    [Fact]
    public async Task RepeatRangeThrowsWhenAlreadyDisposedEvenIfYouDisposeBetweenMoveNextAsync()
    {
        var list = AsyncSequence.Return(1337, 2, 5);

        var repeats = 5;

        foreach (var i in Enumerable.Range(0, await list.CountAsync() * repeats))
        {
            var repeatRange = AsyncSequence.RepeatRange(list, repeats);
            await using var enumerator = repeatRange.GetAsyncEnumerator();

            Assert.True(await AsyncEnumerable.Range(0, i).AllAwaitAsync(async _ => await enumerator.MoveNextAsync()).ConfigureAwait(false));

#pragma warning disable IDISP016 // we test behaviour after Dispose
#pragma warning disable IDISP017 // we test behaviour after Dispose
            await repeatRange.DisposeAsync();
#pragma warning restore IDISP016
#pragma warning restore IDISP017

            await Assert.ThrowsAnyAsync<ObjectDisposedException>(async () => await enumerator.MoveNextAsync());
        }
    }

    [Property]
    public Property TheLengthOfTheGeneratedRepeatRangeIsCorrect(List<int> list, NonNegativeInt count)
        => TheLengthOfTheGeneratedRepeatRangeIsCorrectAsync(list, count.Get)
            .Result
            .ToProperty();

    [Property]
    public Property TheSequenceRepeatsTheGivenNumberOfTimes(List<int> list, NonNegativeInt count)
        => TheSequenceRepeatsTheGivenNumberOfTimesAsync(list.ToAsyncEnumerable(), count.Get)
            .Result
            .ToProperty();

    [Fact]
    public async Task RepeatRangeEnumeratesUnderlyingEnumerableOnlyOnceAsync()
    {
        var sequence = Sequence.Return("Test", "Hello", "Do", "Wait");
        var enumerateOnce = AsyncEnumerateOnce.Create(sequence);

        await using var repeatRange = AsyncSequence.RepeatRange(enumerateOnce, 3);

        await repeatRange.ForEachAsync(NoOperation<string>);
    }

    private static async Task<bool> TheLengthOfTheGeneratedRepeatRangeIsCorrectAsync(List<int> list, int count)
    {
        await using var repeatRange = AsyncSequence.RepeatRange(list.ToAsyncEnumerable(), count);

        var materialized = await repeatRange.ToListAsync();

        return materialized.Count == list.Count * count;
    }

    private static async Task<bool> TheSequenceRepeatsTheGivenNumberOfTimesAsync(IAsyncEnumerable<int> asyncEnumerable, int count)
    {
        await using var repeatRange = AsyncSequence.RepeatRange(asyncEnumerable, count);

        return await repeatRange
            .IsSequenceRepeating(asyncEnumerable)
            .NTimes(count);
    }
}
