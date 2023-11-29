namespace Funcky.Test.TestUtils;

internal sealed class OptionProducer<T>(int retriesNeeded, T result)
    where T : notnull
{
    public int Called { get; private set; }

    public Option<T> Produce()
    {
        Called += 1;

        return Option.FromBoolean(retriesNeeded == (Called - 1), result);
    }
}
