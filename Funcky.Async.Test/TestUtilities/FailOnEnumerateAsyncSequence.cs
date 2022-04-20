using Xunit.Sdk;

namespace Funcky.Async.Test.TestUtilities;

internal sealed class FailOnEnumerateAsyncSequence<T> : IAsyncEnumerable<T>
{
#pragma warning disable 1998, 162
    public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        throw new XunitException("Sequence was unexpectedly enumerated.");
        yield break;
    }
#pragma warning restore 1998, 162
}
