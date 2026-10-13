using Funcky.RetryPolicies;

namespace Funcky.Test.RetryPolicies;

public sealed class ConstantDelayPolicyTest
{
    [Fact]
    public void TheDelayIsTheSameForEveryRetry()
    {
        var delay = TimeSpan.FromMilliseconds(100);
        var retryPolicy = new ConstantDelayPolicy(maxRetries: 3, delay);

        Assert.Equal(
            [delay, delay, delay],
            Enumerable.Range(1, retryPolicy.MaxRetries).Select(retryPolicy.Delay));
    }

    [Fact]
    public void ZeroRetriesAreAllowed()
    {
        var retryPolicy = new ConstantDelayPolicy(maxRetries: 0, TimeSpan.FromMilliseconds(100));

        Assert.Equal(0, retryPolicy.MaxRetries);
    }

    [Fact]
    public void ANegativeNumberOfRetriesIsRejected()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new ConstantDelayPolicy(maxRetries: -1, TimeSpan.FromMilliseconds(100)));

        Assert.Equal("maxRetries", exception.ParamName);
    }
}
