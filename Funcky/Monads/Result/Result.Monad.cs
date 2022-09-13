namespace Funcky.Monads;

public readonly partial struct Result<TValidResult>
{
    [Pure]
    public Result<TResult> Select<TResult>(Func<TValidResult, TResult> selector)
        where TResult : notnull
        => Match(
            error: error => new Result<TResult>(error),
            ok: value => selector(value));

    [Pure]
    public Result<TResult> SelectMany<TResult>(Func<TValidResult, Result<TResult>> selector)
        where TResult : notnull
        => Match(
            error: error => new Result<TResult>(error),
            ok: selector);

    [Pure]
    public Result<TResult> SelectMany<TSelectedResult, TResult>(Func<TValidResult, Result<TSelectedResult>> selector, Func<TValidResult, TSelectedResult, TResult> resultSelector)
        where TSelectedResult : notnull
        where TResult : notnull
        => Match(
            error: error => new Result<TResult>(error),
            ok: result => selector(result).Select(
                option => resultSelector(result, option)));
}
