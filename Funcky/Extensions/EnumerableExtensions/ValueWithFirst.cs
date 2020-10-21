namespace Funcky.Extensions
{
    public readonly struct ValueWithFirst<TValue>
    {
        public ValueWithFirst(TValue value, bool isFirst)
        {
            Value = value;
            IsFirst = isFirst;
        }

        public TValue Value { get; }

        public bool IsFirst { get; }

        public void Deconstruct(out TValue value, out bool isFirst)
        {
            value = Value;
            isFirst = IsFirst;
        }
    }
}
