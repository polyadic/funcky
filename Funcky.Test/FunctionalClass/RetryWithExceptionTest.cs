using FsCheck;
using FsCheck.Xunit;
using Funcky.RetryPolicies;

namespace Funcky.Test.FunctionalClass;

public sealed class RetryWithExceptionTest
{
    [Fact]
    public void ReturnsValueImmediatelyIfProducerDoesNotThrow()
    {
        const int value = 10;
        Assert.Equal(value, Retry(() => value, True, new ThrowOnRetryPolicy()));
    }

    [Fact]
    public void DoesNotRetryIfPredicateReturnsFalse()
    {
        Assert.Throws<ExceptionStub>(() => Retry(Throw<Unit>, False, new ThrowOnRetryPolicy()));
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

        return (value == Retry(Producer, True, new NoDelayRetryPolicy(int.MaxValue))).ToProperty();
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

        Assert.Throws<ExceptionStub>(() => Retry(Producer, True, new NoDelayRetryPolicy(retries.Get)));

        const int firstCall = 1;
        return (firstCall + retries.Get == called).ToProperty();
    }

    private static TResult Throw<TResult>() => throw new ExceptionStub();

    private sealed class ExceptionStub : Exception
    {
    }

    private sealed class ThrowOnRetryPolicy : IRetryPolicy
    {
        public int MaxRetries => 0;

        public TimeSpan Delay(int retryCount) => throw new InvalidOperationException("Retry is disallowed");
    }
}
