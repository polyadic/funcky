using FsCheck;

namespace Funcky.Async.Test;

public class AsyncGenerator<T>
{
    public static Arbitrary<IAsyncEnumerable<T>> GenerateAsyncEnumerable()
        => Arb.Default.List<T>().Generator.Select(list => list.ToAsyncEnumerable()).ToArbitrary();

    public static Arbitrary<Func<T, ValueTask<T>>> GenerateAwaitSelector()
        => Arb.Default.SystemFunc1<T, T>().Generator.Select(ResultToValueTask).ToArbitrary();

    public static Arbitrary<Func<T, CancellationToken, ValueTask<T>>> GenerateAwaitWithCancellationSelector()
        => Arb.Default.SystemFunc1<T, T>().Generator.Select(ResultToValueTaskX).ToArbitrary();

    private static Func<T, ValueTask<T>> ResultToValueTask(Func<T, T> f)
        => value
            => ValueTask.FromResult(f(value));

    private static Func<T, CancellationToken, ValueTask<T>> ResultToValueTaskX(Func<T, T> f)
        => (value, cancellationToken)
            => ValueTask.FromResult(f(value));
}
