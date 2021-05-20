using System;
using System.Diagnostics.Contracts;

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
        public Result<TResult> SelectMany<TResult>(Func<TValidResult, Result<TResult>> selector)
            => Match(
                error: error => new Result<TResult>(error),
                ok: selector);

        [Pure]
        public Result<TResult> SelectMany<TSelectedResult, TResult>(Func<TValidResult, Result<TSelectedResult>> selector, Func<TValidResult, TSelectedResult, TResult> resultSelector)
            => Match(
                error: error => new Result<TResult>(error),
                ok: result => selector(result).Select(
                    maybe => resultSelector(result, maybe)));
    }
}
