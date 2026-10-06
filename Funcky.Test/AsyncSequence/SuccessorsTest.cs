#if ASYNC_SUPPORTED
using Funcky.Async.Test.TestUtilities;

namespace Funcky.Async.Test;

public sealed class SuccessorsTest
{
    [Fact]
    public async Task ReturnsEmptySequenceWhenFirstItemIsNoneAsync()
    {
        await AsyncAssert.Empty(AsyncSequence.Successors(Option<int>.None, ValueTask.FromResult));
    }

    [Fact]
    public async Task ReturnsOnlyTheFirstItemWhenSuccessorFunctionImmediatelyReturnsNoneAsync()
    {
        var first = await AsyncAssert.Single(AsyncSequence.Successors(10, _ => ValueTask.FromResult(Option<int>.None)));
        Assert.Equal(10, first);
    }

    [Fact]
    public async Task SuccessorsWithNonOptionFunctionReturnsEndlessEnumerableAsync()
    {
        const int count = 40;
        Assert.Equal(count, await AsyncSequence.Successors(0, ValueTask.FromResult).Take(count).CountAsync());
    }

    [Fact]
    public async Task SuccessorsReturnsEnumerableThatReturnsValuesBasedOnSeedAsync()
    {
        await AsyncAssert.Equal(
            AsyncEnumerable.Range(0, 10),
            AsyncSequence.Successors(0, i => ValueTask.FromResult(i + 1)).Take(10));
    }

    [Fact]
    public async Task SuccessorsReturnsEnumerableThatReturnsItemUntilNoneIsReturnedFromFuncAsync()
    {
        await AsyncAssert.Equal(
            AsyncEnumerable.Range(0, 11),
            AsyncSequence.Successors(0, i => ValueTask.FromResult(Option.FromBoolean(i < 10, i + 1))));
    }

    [Fact]
    public async Task SuccessorsStopWhenCancelledWhileComputingTheNextElement()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var successors = AsyncSequence.Successors(0, i => ValueTask.FromResult(Option.Some(i + 1)));

        var consumed = new List<int>();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
        {
            await foreach (var item in successors.WithCancellation(cancellationTokenSource.Token))
            {
                consumed.Add(item);
                cancellationTokenSource.Cancel();

                if (consumed.Count > 1)
                {
                    break; // guard against an endless loop when cancellation is ignored
                }
            }
        });

        Assert.Equal([0], consumed);
    }

    [Fact]
    public async Task EndlessSuccessorsStopWhenCancelledWhileComputingTheNextElement()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var successors = AsyncSequence.Successors(0, i => ValueTask.FromResult(i + 1));

        var consumed = new List<int>();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
        {
            await foreach (var item in successors.WithCancellation(cancellationTokenSource.Token))
            {
                consumed.Add(item);
                cancellationTokenSource.Cancel();

                if (consumed.Count > 1)
                {
                    break; // guard against an endless loop when cancellation is ignored
                }
            }
        });

        Assert.Equal([0], consumed);
    }
}
#endif
