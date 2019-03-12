using System;
using Funcky.Monads;

namespace Funcky.Monads
{

    public sealed class Result<TValidResult>
    {
        private readonly TValidResult _result;
        private readonly Exception _error;

        private Result(TValidResult result)
        {
            _result = result;
        }

        private Result(Exception error)
        {
            _error = error;
        }

        public static Result<TValidResult> Ok(TValidResult item)
        {
            return new Result<TValidResult>(item);
        }

        public static Result<TValidResult> Error(Exception item)
        {
            return new Result<TValidResult>(item);
        }

        public Result<TResult> Select<TResult>(Func<TValidResult, TResult> selector)
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }


            return _error is null
                ? Result<TResult>.Ok(selector(_result))
                : Result<TResult>.Error(_error);
        }


        public Result<TResult> SelectMany<TSelectedResult, TResult>(Func<TValidResult, Result<TSelectedResult>> selectedResultSelector, Func<TValidResult, TSelectedResult, TResult> resultSelector)
        {
            if (selectedResultSelector == null)
            {
                throw new ArgumentNullException(nameof(selectedResultSelector));
            }

            if (resultSelector == null)
            {
                throw new ArgumentNullException(nameof(resultSelector));
            }

            Result<TSelectedResult> selectedMaybe = selectedResultSelector(_result);
            if (_error is null)
            {
                return selectedMaybe._error is null
                    ? Result<TResult>.Ok(resultSelector(_result, selectedMaybe._result))
                    : Result<TResult>.Error(selectedMaybe._error);
            }

            return selectedMaybe._error is null
                ? Result<TResult>.Error(_error)
                : Result<TResult>.Error(new ResultCombinationException(_error, selectedMaybe._error));
        }

        public TMatchResult Match<TMatchResult>(Func<TValidResult, TMatchResult> ok, Func<Exception, TMatchResult> error)
        {
            return _error is null
                ? ok(_result) :
                error(_error);
        }

        public override bool Equals(object obj)
        {
            return obj is Result<TValidResult> other
                && Equals(_result, other._result)
                && Equals(_error, other._error);
        }

        public override int GetHashCode()
        {
            return _error is null
                ? _result.GetHashCode()
                : _error.GetHashCode();
        }
    }
}
