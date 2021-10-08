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

        public int Called { get; private set; } = 0;

        public Option<T> Produce()
        {
            Called += 1;

            return Option.FromBoolean(_retriesNeeded == (Called - 1), _result);
        }

        public Task<Option<T>> ProduceAsync() => Task.FromResult(Produce());
    }
}
