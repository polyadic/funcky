using System;
using System.Diagnostics.Contracts;

namespace Funcky.Monads
{
    public readonly partial struct Either<TLeft, TRight>
    {
        [Pure]
        public Either<TLeft, TResult> Select<TResult>(Func<TRight, TResult> selector)
            => Match(
                Either<TLeft, TResult>.Left,
                right => Either<TLeft, TResult>.Right(selector(right)));

        [Pure]
        public Either<TLeft, TResult> SelectMany<TEither, TResult>(Func<TRight, Either<TLeft, TEither>> eitherSelector, Func<TRight, TEither, TResult> resultSelector)
            => Match(
                Either<TLeft, TResult>.Left,
                right => eitherSelector(right).Select(
                    selectedRight => resultSelector(right, selectedRight)));
    }
}
