namespace Funcky.Async.Test.TestUtilities;

internal sealed class OptionProducer<T>
    where T : notnull
{
    private readonly T _result;
    private readonly int _retriesNeeded;

    public OptionProducer(int retriesNeeded, T result)
    {
        _retriesNeeded = retriesNeeded;
        _result = result;
    }

    public int Called { get; private set; }

    public ValueTask<Option<T>> ProduceAsync()
    {
        Called += 1;

        return ValueTask.FromResult(Option.FromBoolean(_retriesNeeded == (Called - 1), _result));
    }
}
