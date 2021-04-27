using Funcky.Monads;

namespace Funcky.Extensions
{
    public readonly struct ValueWithPrevious<TValue>
        where TValue : notnull
    {
        public readonly TValue Value;

        public readonly Option<TValue> Previous;

        public ValueWithPrevious(TValue value, Option<TValue> previous) => (Value, Previous) = (value, previous);

        public void Deconstruct(out TValue value, out Option<TValue> previous) => (value, previous) = (Value, Previous);
    }
}
