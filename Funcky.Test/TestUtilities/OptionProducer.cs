namespace Funcky.Test.TestUtilities;

internal sealed class OptionProducer<T>(int retriesNeeded, T result)
    where T : notnull
{
    public int Called { get; private set; }

    public Option<T> Produce()
    {
        Called += 1;

        return (retriesNeeded == (Called - 1)).Then(result);
    }

#if ASYNC_SUPPORTED
    public ValueTask<Option<T>> ProduceAsync()
    {
        Called += 1;

        return ValueTask.FromResult((retriesNeeded == (Called - 1)).Then(result));
    }
#endif
}
