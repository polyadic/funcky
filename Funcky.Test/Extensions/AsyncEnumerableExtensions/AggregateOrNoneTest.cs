#if ASYNC_SUPPORTED
// ReSharper disable PossibleMultipleEnumeration
using System.Runtime.CompilerServices;
using FsCheck;
using FsCheck.Fluent;

namespace Funcky.Async.Test.Extensions.AsyncEnumerableExtensions;

public sealed class AggregateOrNoneTest
{
    [FunckyAsyncProperty]
    public Property AggregateOrNoneAsyncGivesTheSameResultAsAggregateAsyncForNonEmptySequences(IAsyncEnumerable<int> sequence, Func<int, int, int> func)
        => (sequence.AnyAsync().Result
                ? Option.Some(sequence.AggregateAsync(func).Result) == sequence.AggregateOrNoneAsync(func).Result
                : sequence.AggregateOrNoneAsync(func).Result == Option<int>.None)
            .ToProperty();

    [FunckyAsyncProperty]
    public Property AggregateOrNoneAsyncWithAnAsynchronousAccumulatorGivesTheSameResultAsTheSynchronousOne(IAsyncEnumerable<int> sequence, Func<int, int, int> func)
        => (sequence.AggregateOrNoneAsync(func).Result
            == sequence.AggregateOrNoneAsync((a, b, _) => new ValueTask<int>(func(a, b))).Result).ToProperty();

    [Fact]
    public async Task AggregateOrNoneAsyncReturnsNoneForAnEmptySequence()
        => FunctionalAssert.None(await AsyncEnumerable.Empty<int>().AggregateOrNoneAsync((a, b) => a + b));

    [Fact]
    public async Task AggregateOrNoneAsyncReturnsTheOnlyElementWithoutInvokingTheAccumulator()
    {
        var accumulatorInvoked = false;

        var result = await AsyncSequence.Return(42).AggregateOrNoneAsync((a, b) =>
        {
            accumulatorInvoked = true;
            return a + b;
        });

        FunctionalAssert.Some(42, result);
        Assert.False(accumulatorInvoked);
    }

    [Fact]
    public async Task AggregateOrNoneAsyncFoldsFromTheLeft()
        => FunctionalAssert.Some("((a,b),c)", await AsyncSequence.Return("a", "b", "c").AggregateOrNoneAsync((a, b) => $"({a},{b})"));

    [Fact]
    public async Task AggregateOrNoneAsyncPassesTheCancellationTokenToTheSourceAndTheAccumulator()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        var tokenSeenBySource = CancellationToken.None;

        var result = await SequenceRecordingTheToken(token => tokenSeenBySource = token)
            .AggregateOrNoneAsync(
                (a, b, token) =>
                {
                    Assert.Equal(cancellationToken, token);
                    return new ValueTask<int>(a + b);
                },
                cancellationToken);

        FunctionalAssert.Some(6, result);
        Assert.Equal(cancellationToken, tokenSeenBySource);
    }

    private static async IAsyncEnumerable<int> SequenceRecordingTheToken(Action<CancellationToken> record, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        record(cancellationToken);
        await Task.Yield();
        yield return 1;
        yield return 2;
        yield return 3;
    }
}
#endif
