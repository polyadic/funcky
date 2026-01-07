using static Funcky.ValueTaskFactory;

namespace Funcky.Monads;

public static class ResultAsyncExtensions
{
    [Pure]
    public static IAsyncEnumerable<Result<T>> Traverse<TValidResult, T>(
        this Result<TValidResult> result,
        Func<TValidResult, IAsyncEnumerable<T>> selector)
        where TValidResult : notnull
        where T : notnull
        => result.Select(selector).Sequence();

    [Pure]
    public static IAsyncEnumerable<Result<TValidResult>> Sequence<TValidResult>(
        this Result<IAsyncEnumerable<TValidResult>> result)
        where TValidResult : notnull
        => result.Match(
            error: static error => AsyncSequence.Return(Result<TValidResult>.Error(error)),
            ok: static ok => ok.Select(Result.Return));

    [Pure]
    public static Task<Result<T>> Traverse<TValidResult, T>(
        this Result<TValidResult> result,
        Func<TValidResult, Task<T>> selector)
        where TValidResult : notnull
        where T : notnull
        => result.Select(selector).Sequence();

    [Pure]
    public static Task<Result<TValidResult>> Sequence<TValidResult>(
        this Result<Task<TValidResult>> result)
        where TValidResult : notnull
        => result.Match(
            error: static error => Task.FromResult(Result<TValidResult>.Error(error)),
            ok: static async ok => Result.Return(await ok.ConfigureAwait(false)));

    [Pure]
    public static ValueTask<Result<T>> Traverse<TValidResult, T>(
        this Result<TValidResult> result,
        Func<TValidResult, ValueTask<T>> selector)
        where TValidResult : notnull
        where T : notnull
        => result.Select(selector).Sequence();

    [Pure]
    public static ValueTask<Result<TValidResult>> Sequence<TValidResult>(
        this Result<ValueTask<TValidResult>> result)
        where TValidResult : notnull
        => result.Match<ValueTask<Result<TValidResult>>>(
            error: static error => ValueTaskFromResult(Result<TValidResult>.Error(error)),
            ok: static async ok => Result.Return(await ok.ConfigureAwait(false)));
}
