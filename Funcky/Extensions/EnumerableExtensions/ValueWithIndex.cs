namespace Funcky.Extensions
{
    public readonly struct ValueWithIndex<TValue>
    {
        public ValueWithIndex(TValue value, int index) => (Value, Index) = (value, index);

        public TValue Value { get; }

        public int Index { get; }

        public void Deconstruct(out TValue value, out int index) => (value, index) = (Value, Index);

        public static ValueWithIndex<TValue> Create(TValue value, int index)
            => new(value, index);
    }
}
