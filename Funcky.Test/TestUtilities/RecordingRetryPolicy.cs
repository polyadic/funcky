using Funcky.RetryPolicies;

namespace Funcky.Test.TestUtilities;

/// <summary>A retry policy without delays that records the retry counts it is asked to provide a delay for.</summary>
internal sealed class RecordingRetryPolicy(int maxRetries) : IRetryPolicy
{
    private readonly List<int> _requestedRetryCounts = [];

    public int MaxRetries => maxRetries;

    public IReadOnlyList<int> RequestedRetryCounts => _requestedRetryCounts;

    public TimeSpan Delay(int retryCount)
    {
        _requestedRetryCounts.Add(retryCount);
        return TimeSpan.Zero;
    }
}
