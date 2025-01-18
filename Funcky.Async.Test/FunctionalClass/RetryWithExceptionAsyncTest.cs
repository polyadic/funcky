using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
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

    private static TResult Throw<TResult>() => throw new ExceptionStub();

    private sealed class ExceptionStub : Exception;

    private sealed class ThrowOnRetryPolicy : IRetryPolicy
    {
        public int MaxRetries => 0;

        public TimeSpan Delay(int retryCount) => throw new InvalidOperationException("Retry is disallowed");
    }
}
