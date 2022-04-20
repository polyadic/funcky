namespace Funcky.Extensions;

public readonly struct ValueWithPrevious<TValue>
    where TValue : notnull
{
    public ValueWithPrevious(TValue value, Option<TValue> previous) => (Value, Previous) = (value, previous);

    public TValue Value { get; }

    public Option<TValue> Previous { get; }

    public void Deconstruct(out TValue value, out Option<TValue> previous) => (value, previous) = (Value, Previous);
}
