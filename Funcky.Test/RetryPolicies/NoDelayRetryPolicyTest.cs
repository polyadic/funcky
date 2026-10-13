using Funcky.RetryPolicies;

namespace Funcky.Test.RetryPolicies;

public sealed class NoDelayRetryPolicyTest
{
    [Fact]
    public void TheDelayIsZeroForEveryRetry()
    {
        var retryPolicy = new NoDelayRetryPolicy(maxRetries: 3);

        Assert.All(Enumerable.Range(1, retryPolicy.MaxRetries).Select(retryPolicy.Delay), delay => Assert.Equal(TimeSpan.Zero, delay));
    }

    [Fact]
    public void ANegativeNumberOfRetriesIsRejected()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new NoDelayRetryPolicy(maxRetries: -1));

        Assert.Equal("maxRetries", exception.ParamName);
    }
}
