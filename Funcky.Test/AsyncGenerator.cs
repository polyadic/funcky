#if ASYNC_SUPPORTED
using FsCheck;
using FsCheck.Fluent;

namespace Funcky.Async.Test;

internal static class AsyncGenerator
{
    public static Arbitrary<IAsyncEnumerable<T>> GenerateAsyncEnumerable<T>(IArbMap map)
        => map.GeneratorFor<List<T>>().Select(list => list.ToAsyncEnumerable()).ToArbitrary();

    public static Arbitrary<AwaitSelectorWithCancellation<T>> GenerateAwaitWithCancellationSelector<T>(IArbMap map)
        => map.GeneratorFor<Func<T, T>>().Select(ResultToValueTaskX).ToArbitrary();

    private static AwaitSelectorWithCancellation<T> ResultToValueTaskX<T>(Func<T, T> f)
        => new((value, _) => ValueTask.FromResult(f(value)));
}

public sealed record AwaitSelectorWithCancellation<T>(Func<T, CancellationToken, ValueTask<T>> Get);
#endif
