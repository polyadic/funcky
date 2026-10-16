#if ASYNC_SUPPORTED
namespace Funcky.Async.Test.TestUtilities;

/// <summary>Counts how many enumerators were handed out and how many of them were disposed.</summary>
internal sealed class DisposeTrackingAsyncSequence<T>(IAsyncEnumerable<T> source) : IAsyncEnumerable<T>
{
    public int EnumeratorsCreated { get; private set; }

    public int EnumeratorsDisposed { get; private set; }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        EnumeratorsCreated++;
#pragma warning disable IDISP004 // The enumerator is owned by the TrackingEnumerator.
        return new TrackingEnumerator(this, source.GetAsyncEnumerator(cancellationToken));
#pragma warning restore IDISP004
    }

    private sealed class TrackingEnumerator(DisposeTrackingAsyncSequence<T> owner, IAsyncEnumerator<T> inner) : IAsyncEnumerator<T>
    {
        public T Current => inner.Current;

        public ValueTask<bool> MoveNextAsync() => inner.MoveNextAsync();

        public ValueTask DisposeAsync()
        {
            owner.EnumeratorsDisposed++;
#pragma warning disable IDISP007 // The inner enumerator is owned by this wrapper.
            return inner.DisposeAsync();
#pragma warning restore IDISP007
        }
    }
}
#endif
