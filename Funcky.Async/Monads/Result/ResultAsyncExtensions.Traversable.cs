using System.Diagnostics.CodeAnalysis;

namespace Funcky.Monads;

public static class ResultAsyncExtensions
{
    [Pure]
    public static IAsyncEnumerable<Result<T>> Traverse<TValidResult, T>(
        this Result<TValidResult> result,
        Func<TValidResult, IAsyncEnumerable<T>> selector)
        => result.Select(selector).Sequence();

    [Pure]
    public static IAsyncEnumerable<Result<TValidResult>> Sequence<TValidResult>(
        this Result<IAsyncEnumerable<TValidResult>> result)
        => result.Match(
            error: static error => AsyncSequence.Return(Result<TValidResult>.Error(error)),
            ok: static ok => ok.Select(Result.Return));

    [Pure]
    public static Task<Result<T>> Traverse<TValidResult, T>(
        this Result<TValidResult> result,
        Func<TValidResult, Task<T>> selector)
        => result.Select(selector).Sequence();

    [Pure]
    public static Task<Result<TValidResult>> Sequence<TValidResult>(
        this Result<Task<TValidResult>> result)
        => result.Match(
            error: static error => Task.FromResult(Result<TValidResult>.Error(error)),
            ok: static async ok => Result.Return(await ok.ConfigureAwait(false)));

    [Pure]
    public static ValueTask<Result<T>> Traverse<TValidResult, T>(
        this Result<TValidResult> result,
        Func<TValidResult, ValueTask<T>> selector)
        => result.Select(selector).Sequence();

    [Pure]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1114:Parameter list should follow declaration", Justification = "False positive.")]
    public static ValueTask<Result<TValidResult>> Sequence<TValidResult>(
        this Result<ValueTask<TValidResult>> result)
        => result.Match<ValueTask<Result<TValidResult>>>(
#if VALUE_TASK_HAS_FROM_RESULT
            error: static error => ValueTask.FromResult(Result<TValidResult>.Error(error)),
#else
            error: static error => new ValueTask<Result<TValidResult>>(Result<TValidResult>.Error(error)),
#endif
            ok: static async ok => Result.Return(await ok.ConfigureAwait(false)));
}
