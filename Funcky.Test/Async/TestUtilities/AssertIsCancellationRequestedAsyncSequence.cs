#pragma warning disable CS1998

namespace Funcky.Test.Async.TestUtilities;

internal sealed class AssertIsCancellationRequestedAsyncSequence<T> : IAsyncEnumerable<T>
{
    public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        Assert.True(cancellationToken.IsCancellationRequested, "cancellationToken.IsCancellationRequested");
        yield break;
    }
}
