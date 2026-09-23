using Funcky.RetryPolicies;

namespace Funcky.Test.RetryPolicies;

public sealed class ExponentialBackOffRetryPolicyTest
{
    [Fact]
    public void TheDelayBeforeTheFirstRetryIsTheFirstDelay()
    {
        var firstDelay = TimeSpan.FromMilliseconds(100);
        var retryPolicy = new ExponentialBackOffRetryPolicy(maxRetries: 3, firstDelay);

        Assert.Equal(firstDelay, retryPolicy.Delay(1));
    }

    [Fact]
    public void TheDelayGrowsExponentiallyWithTheRetryCount()
    {
        var firstDelay = TimeSpan.FromMilliseconds(100);
        var retryPolicy = new ExponentialBackOffRetryPolicy(maxRetries: 3, firstDelay);

        Assert.Equal(
            [TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(150), TimeSpan.FromMilliseconds(225)],
            Enumerable.Range(1, retryPolicy.MaxRetries).Select(retryPolicy.Delay));
    }
}
