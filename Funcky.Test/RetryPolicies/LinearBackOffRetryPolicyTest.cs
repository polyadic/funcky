using Funcky.RetryPolicies;

namespace Funcky.Test.RetryPolicies;

public sealed class LinearBackOffRetryPolicyTest
{
    [Fact]
    public void TheDelayBeforeTheFirstRetryIsTheFirstDelay()
    {
        var firstDelay = TimeSpan.FromMilliseconds(100);
        var retryPolicy = new LinearBackOffRetryPolicy(maxRetries: 3, firstDelay);

        Assert.Equal(firstDelay, retryPolicy.Delay(1));
    }

    [Fact]
    public void TheDelayGrowsLinearlyWithTheRetryCount()
    {
        var firstDelay = TimeSpan.FromMilliseconds(100);
        var retryPolicy = new LinearBackOffRetryPolicy(maxRetries: 3, firstDelay);

        Assert.Equal(
            [TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(200), TimeSpan.FromMilliseconds(300)],
            Enumerable.Range(1, retryPolicy.MaxRetries).Select(retryPolicy.Delay));
    }

    [Fact]
    public void ZeroRetriesAreAllowed()
    {
        var retryPolicy = new LinearBackOffRetryPolicy(maxRetries: 0, TimeSpan.FromMilliseconds(100));

        Assert.Equal(0, retryPolicy.MaxRetries);
    }

    [Fact]
    public void ANegativeNumberOfRetriesIsRejected()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new LinearBackOffRetryPolicy(maxRetries: -1, TimeSpan.FromMilliseconds(100)));

        Assert.Equal("maxRetries", exception.ParamName);
    }
}
