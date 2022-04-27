namespace Funcky.Async.Test.TestUtilities;

internal sealed class MaybeProducer<T>
    where T : notnull
{
    private readonly T _result;
    private readonly int _retriesNeeded;

    public MaybeProducer(int retriesNeeded, T result)
    {
        _retriesNeeded = retriesNeeded;
        _result = result;
    }

    public int Called { get; private set; }

    public ValueTask<Option<T>> ProduceAsync()
    {
        Called += 1;

        return ValueTask.FromResult(ProduceResult());
    }

    private Option<T> ProduceResult()
        => IsReady()
            ? _result
            : Option<T>.None;

    private bool IsReady()
        => _retriesNeeded == Called - 1;
}
