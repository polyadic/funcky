namespace Funcky.Monads
{
    public readonly partial struct Result<TValidResult>
    {
        [Pure]
        public Result<TResult> Select<TResult>(Func<TValidResult, TResult> selector)
            => Match(
                error: error => new Result<TResult>(error),
                ok: value => Result.Ok(selector(value)));

        [Pure]
        public Result<TResult> SelectMany<TResult>(Func<TValidResult, Result<TResult>> resultSelector)
            => Match(
                error: error => new Result<TResult>(error),
                ok: resultSelector);

        [Pure]
        public Result<TResult> SelectMany<TSelectedResult, TResult>(Func<TValidResult, Result<TSelectedResult>> selectedResultSelector, Func<TValidResult, TSelectedResult, TResult> resultSelector)
            => Match(
                error: error => new Result<TResult>(error),
                ok: result => selectedResultSelector(result).Select(
                    maybe => resultSelector(result, maybe)));
    }
}
