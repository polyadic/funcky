namespace Funcky.Test.TestUtilities;

internal sealed class OptionProducer<T>(int retriesNeeded, T result)
    where T : notnull
{
    public int Called { get; private set; }

    public Option<T> Produce()
    {
        Called += 1;

        return Option.FromBoolean(retriesNeeded == (Called - 1), result);
    }

#if INTEGRATED_ASYNC
    public ValueTask<Option<T>> ProduceAsync()
    {
        Called += 1;

        return ValueTask.FromResult(Option.FromBoolean(retriesNeeded == (Called - 1), result));
    }
#endif
}
