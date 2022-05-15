namespace Funcky.Monads;

public static class ResultExtensions
{
    [Pure]
    public static Either<TLeft, Result<TRight>> Traverse<TValidResult, TLeft, TRight>(
        this Result<TValidResult> result,
        Func<TValidResult, Either<TLeft, TRight>> selector)
        => result.Select(selector).Sequence();

    [Pure]
    public static Either<TLeft, Result<TValidResult>> Sequence<TLeft, TValidResult>(
        this Result<Either<TLeft, TValidResult>> result)
        => result.Match(
            error: static error => Either<TLeft>.Return(Result<TValidResult>.Error(error)),
            ok: static ok => ok.Select(Result.Return));

    [Pure]
    public static Option<Result<TItem>> Traverse<TValidResult, TItem>(
        this Result<TValidResult> result,
        Func<TValidResult, Option<TItem>> selector)
        where TItem : notnull
        => result.Select(selector).Sequence();

    [Pure]
    public static Option<Result<TValidResult>> Sequence<TValidResult>(
        this Result<Option<TValidResult>> result)
        where TValidResult : notnull
        => result.Match(
            error: static error => Option.Return(Result<TValidResult>.Error(error)),
            ok: static ok => ok.Select(Result.Return));

    [Pure]
    public static Lazy<Result<T>> Traverse<TValidResult, T>(
        this Result<TValidResult> result,
        Func<TValidResult, Lazy<T>> selector)
        => result.Select(selector).Sequence();

    [Pure]
    public static Lazy<Result<TValidResult>> Sequence<TValidResult>(
        this Result<Lazy<TValidResult>> result)
        => result.Match(
            error: static error => Lazy.Return(Result<TValidResult>.Error(error)),
            ok: static ok => ok.Select(Result.Return));

    [Pure]
    public static IEnumerable<Result<T>> Traverse<TValidResult, T>(
        this Result<TValidResult> result,
        Func<TValidResult, IEnumerable<T>> selector)
        => result.Select(selector).Sequence();

    [Pure]
    public static IEnumerable<Result<TValidResult>> Sequence<TValidResult>(
        this Result<IEnumerable<TValidResult>> result)
        => result.Match(
            error: static error => Funcky.Sequence.Return(Result<TValidResult>.Error(error)),
            ok: static ok => ok.Select(Result.Return));

    [Pure]
    public static Reader<TEnvironment, Result<TResult>> Traverse<TValidResult, TEnvironment, TResult>(
        this Result<TValidResult> result,
        Func<TValidResult, Reader<TEnvironment, TResult>> selector)
        => result.Select(selector).Sequence();

    [Pure]
    public static Reader<TEnvironment, Result<TValidResult>> Sequence<TEnvironment, TValidResult>(
        this Result<Reader<TEnvironment, TValidResult>> result)
        => result.Match(
            error: static error => Reader<TEnvironment>.Return(Result<TValidResult>.Error(error)),
            ok: static ok => ok.Select(Result.Return));
}
