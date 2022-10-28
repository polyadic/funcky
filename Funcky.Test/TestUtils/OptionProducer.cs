namespace Funcky.Test.TestUtils;

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

    public Option<T> Produce()
    {
        Called += 1;

        return Option.FromBoolean(_retriesNeeded == (Called - 1), _result);
    }
}
