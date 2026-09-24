using FsCheck;
using FsCheck.Fluent;

namespace Funcky.Async.Test;

internal static class AsyncGenerator
{
    public static Arbitrary<IAsyncEnumerable<T>> GenerateAsyncEnumerable<T>(IArbMap map)
        => map.GeneratorFor<List<T>>().Select(list => list.ToAsyncEnumerable()).ToArbitrary();

    public static Arbitrary<AwaitSelector<T>> GenerateAwaitSelector<T>(IArbMap map)
        => map.GeneratorFor<Func<T, T>>().Select(ResultToValueTask).ToArbitrary();

    public static Arbitrary<AwaitSelectorWithCancellation<T>> GenerateAwaitWithCancellationSelector<T>(IArbMap map)
        => map.GeneratorFor<Func<T, T>>().Select(ResultToValueTaskX).ToArbitrary();

    private static AwaitSelector<T> ResultToValueTask<T>(Func<T, T> f)
        => new(value => ValueTask.FromResult(f(value)));

    private static AwaitSelectorWithCancellation<T> ResultToValueTaskX<T>(Func<T, T> f)
        => new((value, _) => ValueTask.FromResult(f(value)));
}

public sealed record AwaitSelector<T>(Func<T, ValueTask<T>> Get);

public sealed record AwaitSelectorWithCancellation<T>(Func<T, CancellationToken, ValueTask<T>> Get);
