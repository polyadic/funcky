using System.Collections;

namespace Funcky.Test.TestUtilities;

/// <summary>Counts how many enumerators were handed out and how many of them were disposed.</summary>
internal sealed class DisposeTrackingSequence<T>(IEnumerable<T> source) : IEnumerable<T>
{
    public int EnumeratorsCreated { get; private set; }

    public int EnumeratorsDisposed { get; private set; }

    public IEnumerator<T> GetEnumerator()
    {
        EnumeratorsCreated++;
#pragma warning disable IDISP004 // The enumerator is owned by the TrackingEnumerator.
        return new TrackingEnumerator(this, source.GetEnumerator());
#pragma warning restore IDISP004
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private sealed class TrackingEnumerator(DisposeTrackingSequence<T> owner, IEnumerator<T> inner) : IEnumerator<T>
    {
        public T Current => inner.Current;

        object? IEnumerator.Current => Current;

        public bool MoveNext() => inner.MoveNext();

        public void Reset() => inner.Reset();

        public void Dispose()
        {
            owner.EnumeratorsDisposed++;
#pragma warning disable IDISP007 // The inner enumerator is owned by this wrapper.
            inner.Dispose();
#pragma warning restore IDISP007
        }
    }
}
