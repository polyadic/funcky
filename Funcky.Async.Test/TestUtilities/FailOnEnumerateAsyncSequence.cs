using Xunit.Sdk;

namespace Funcky.Async.Test.TestUtilities;

internal sealed class FailOnEnumerateAsyncSequence<T> : IAsyncEnumerable<T>
{
    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        throw new XunitException("Sequence was unexpectedly enumerated.");
    }
}
