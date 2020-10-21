namespace Funcky.Extensions
{
    public readonly struct ValueWithLast<TValue>
    {
        public ValueWithLast(TValue value, bool isLast)
        {
            Value = value;
            IsLast = isLast;
        }

        public TValue Value { get; }

        public bool IsLast { get; }

        public void Deconstruct(out TValue value, out bool isLast)
        {
            value = Value;
            isLast = IsLast;
        }
    }
}
