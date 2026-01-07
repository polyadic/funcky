#if !NET48

namespace Funcky.Test.TestUtils;

internal sealed class OptionProducerAsync<T>(int retriesNeeded, T result)
    where T : notnull
{
    public int Called { get; private set; }

    public ValueTask<Option<T>> ProduceAsync()
    {
        Called += 1;

        return ValueTask.FromResult(Option.FromBoolean(retriesNeeded == (Called - 1), result));
    }
}

#endif
