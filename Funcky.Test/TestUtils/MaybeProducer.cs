namespace Funcky.Test.TestUtils
{
    internal sealed class MaybeProducer<T>
        where T : notnull
    {
        private readonly T _result;
        private readonly int _retriesNeeded;

        public MaybeProducer(int retriesNeeded, T result)
        {
            _retriesNeeded = retriesNeeded;
            _result = result;
        }

        public int Called { get; private set; }

        public Option<T> Produce()
        {
            Called += 1;

            return Option.FromBoolean(_retriesNeeded == (Called - 1), _result);
        }

        private bool IsReady()
            => _retriesNeeded == Called - 1;
    }
}
