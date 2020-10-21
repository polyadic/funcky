namespace Funcky.Extensions
{
    public readonly struct ValueWithIndex<TValue>
    {
        public ValueWithIndex(TValue value, int index)
        {
            Value = value;
            Index = index;
        }

        public TValue Value { get; }

        public int Index { get; }

        public void Deconstruct(out TValue value, out int index)
        {
            value = Value;
            index = Index;
        }
    }
}
