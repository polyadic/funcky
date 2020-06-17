using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Funcky.Monads
{
    public readonly struct Result<TValidResult>
    {
        private const int SkipLowestStackFrame = 1;

        private readonly TValidResult _result;
        private readonly Exception _error;

        internal Result(TValidResult result)
        {
            _result = result;
            _error = null!;
        }

        private Result(Exception error)
        {
            _result = default!;
            _error = error;
        }

        [Pure]
        public static bool operator ==(Result<TValidResult> lhs, Result<TValidResult> rhs)
            => lhs.Equals(rhs);

        [Pure]
        public static bool operator !=(Result<TValidResult> lhs, Result<TValidResult> rhs)
            => !lhs.Equals(rhs);

        [Pure]
        public static Result<TValidResult> Error(Exception item)
        {
            item.SetStackTrace(new StackTrace(SkipLowestStackFrame, true));

            return new Result<TValidResult>(item);
        }

        [Pure]
        public Result<TResult> Select<TResult>(Func<TValidResult, TResult> selector)
            => _error is null
                ? Result.Ok(selector(_result))
                : Result<TResult>.Error(_error);

        [Pure]
        public Result<TResult> SelectMany<TSelectedResult, TResult>(Func<TValidResult, Result<TSelectedResult>> selectedResultSelector, Func<TValidResult, TSelectedResult, TResult> resultSelector)
        {
            var selectedMaybe = selectedResultSelector(_result);
            if (_error is null)
            {
                return selectedMaybe._error is null
                    ? Result.Ok(resultSelector(_result, selectedMaybe._result))
                    : Result<TResult>.Error(selectedMaybe._error);
            }

            return selectedMaybe._error is null
                ? Result<TResult>.Error(_error)
                : Result<TResult>.Error(new ResultCombinationException(_error, selectedMaybe._error));
        }

        [Pure]
        public TMatchResult Match<TMatchResult>(Func<TValidResult, TMatchResult> ok, Func<Exception, TMatchResult> error)
            => _error is null
                ? ok(_result)
                : error(_error);

        public void Match(Action<TValidResult> ok, Action<Exception> error)
        {
            if (_error is null)
            {
                ok(_result);
            }
            else
            {
                error(_error);
            }
        }

        [Pure]
        public override bool Equals(object obj)
            => obj is Result<TValidResult> other
                 && Equals(_result, other._result)
                 && Equals(_error, other._error);

        [Pure]
        public override int GetHashCode()
            => Match(
                ok: result => result?.GetHashCode(),
                error: error => error.GetHashCode()) ?? 0;
    }

    public static class Result
    {
        [Pure]

        public static Result<TValidResult> Ok<TValidResult>(TValidResult item) => new Result<TValidResult>(item);
    }
}
