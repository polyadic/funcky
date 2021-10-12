namespace Funcky.DataTypes
{
    public readonly struct ValueWithFirst<TValue>
    {
        public ValueWithFirst(TValue value, bool isFirst) => (Value, IsFirst) = (value, isFirst);

        public TValue Value { get; }

        public bool IsFirst { get; }

        public void Deconstruct(out TValue value, out bool isFirst) => (value, isFirst) = (Value, IsFirst);
    }
}
