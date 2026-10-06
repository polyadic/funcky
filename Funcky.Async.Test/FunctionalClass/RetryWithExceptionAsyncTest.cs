using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using Funcky.Async.Test.TestUtilities;
using Funcky.RetryPolicies;
using static Funcky.AsyncFunctional;

namespace Funcky.Async.Test.FunctionalClass;

public sealed class RetryWithExceptionAsyncTest
{
    [Fact]
    public async Task ReturnsValueImmediatelyIfProducerDoesNotThrow()
    {
        const int value = 10;
        Assert.Equal(value, await RetryAsync(() => value, True, new ThrowOnRetryPolicy()));
    }

    [Fact]
    public async Task DoesNotRetryIfPredicateReturnsFalse()
    {
        await Assert.ThrowsAsync<ExceptionStub>(async () => await RetryAsync(Throw<Unit>, False, new ThrowOnRetryPolicy()));
    }

    [Property]
    public Property RetriesProducerUntilItNoLongerThrows(NonNegativeInt callsUntilValueIsReturned)
    {
        const int value = 42;

        var called = 0;
        int Producer()
            => called++ == callsUntilValueIsReturned.Get
                ? value
                : throw new ExceptionStub();

        return (RetryAsync(Producer, True, new NoDelayRetryPolicy(int.MaxValue)).Result == value).ToProperty();
    }

    [Property]
    public Property RetriesProducerUntilRetriesAreExhausted(NonNegativeInt retries)
    {
        var called = 0;
        Unit Producer()
        {
            called++;
            throw new ExceptionStub();
        }

        Assert.Throws<ExceptionStub>(() => RetryAsync(Producer, True, new NoDelayRetryPolicy(retries.Get)).Result);

        const int firstCall = 1;
        return (called == firstCall + retries.Get).ToProperty();
    }

    [Fact]
    public async Task RequestsTheDelaysWithOneBasedRetryCounts()
    {
        var retryPolicy = new RecordingRetryPolicy(3);

        await Assert.ThrowsAsync<ExceptionStub>(async () => await RetryAsync(Throw<Unit>, True, retryPolicy));

        Assert.Equal([1, 2, 3], retryPolicy.RequestedRetryCounts);
    }

    [Fact]
    public async Task DoesNotRetryWhenTheProducerWasCancelled()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var called = 0;

        ValueTask<Unit> Producer()
        {
            called++;
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Token.ThrowIfCancellationRequested();
            return ValueTask.FromResult(Unit.Value);
        }

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await RetryAwaitAsync(Producer, True, new NoDelayRetryPolicy(5), cancellationTokenSource.Token));
        Assert.Equal(1, called);
    }

    [Fact]
    public async Task ThrowsImmediatelyWhenAlreadyCancelled()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await RetryAsync(Throw<Unit>, True, new NoDelayRetryPolicy(5), cancellationTokenSource.Token));
    }

    [Fact]
    public async Task TheProducerReceivesTheCancellationToken()
    {
        using var cancellationTokenSource = new CancellationTokenSource();

        var receivedToken = await RetryAwaitAsync(token => ValueTask.FromResult(token), True, new ThrowOnRetryPolicy(), cancellationTokenSource.Token);

        Assert.Equal(cancellationTokenSource.Token, receivedToken);
    }

    [Fact]
    public async Task TheActionReceivesTheCancellationToken()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var receivedToken = CancellationToken.None;

        await RetryAwaitAsync(
            token =>
            {
                receivedToken = token;
                return ValueTask.CompletedTask;
            },
            True,
            new ThrowOnRetryPolicy(),
            cancellationTokenSource.Token);

        Assert.Equal(cancellationTokenSource.Token, receivedToken);
    }

    [Fact]
    public async Task TheProducerReceivesTheCancellationTokenOnEveryRetry()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var receivedTokens = new List<CancellationToken>();

        await Assert.ThrowsAsync<ExceptionStub>(async () => await RetryAwaitAsync<Unit>(
            token =>
            {
                receivedTokens.Add(token);
                throw new ExceptionStub();
            },
            True,
            new NoDelayRetryPolicy(2),
            cancellationTokenSource.Token));

        Assert.Equal([cancellationTokenSource.Token, cancellationTokenSource.Token, cancellationTokenSource.Token], receivedTokens);
    }

    private static TResult Throw<TResult>() => throw new ExceptionStub();

    private sealed class ExceptionStub : Exception;

    private sealed class ThrowOnRetryPolicy : IRetryPolicy
    {
        public int MaxRetries => 0;

        public TimeSpan Delay(int retryCount) => throw new InvalidOperationException("Retry is disallowed");
    }
}
