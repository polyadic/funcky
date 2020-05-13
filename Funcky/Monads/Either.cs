using System;

namespace Funcky.Monads
{
    public readonly struct Either<TLeft, TRight>
    {
        private readonly TLeft _left;
        private readonly TRight _right;
        private readonly bool _isRight;

        private Either(TLeft left)
        {
            _left = left;
            _right = default(TRight);
            _isRight = false;
        }

        private Either(TRight right)
        {
            _left = default(TLeft);
            _right = right;
            _isRight = true;
        }

        public static Either<TLeft, TRight> Left(TLeft left)
        {
            return new Either<TLeft, TRight>(left);
        }

        public static Either<TLeft, TRight> Right(TRight right)
        {
            return new Either<TLeft, TRight>(right);
        }

        public Either<TLeft, TResult> Select<TResult>(Func<TRight, TResult> selector)
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return _isRight
                ? Either<TLeft, TResult>.Right(selector(_right))
                : Either<TLeft, TResult>.Left(_left);
        }

        public Either<TLeft, TResult> SelectMany<TEither, TResult>(Func<TRight, Either<TLeft, TEither>> eitherSelector, Func<TRight, TEither, TResult> resultSelector)
        {
            if (eitherSelector == null)
            {
                throw new ArgumentNullException(nameof(eitherSelector));
            }

            if (resultSelector == null)
            {
                throw new ArgumentNullException(nameof(resultSelector));
            }

            Either<TLeft, TEither> selectedEither = eitherSelector(_right);
            if (_isRight)
            {
                return selectedEither._isRight
                    ? Either<TLeft, TResult>.Right(resultSelector(_right, selectedEither._right))
                    : Either<TLeft, TResult>.Left(selectedEither._left);
            }

            return Either<TLeft, TResult>.Left(_left);
        }

        public TMatchResult Match<TMatchResult>(Func<TLeft, TMatchResult> left, Func<TRight, TMatchResult> right)
        {
            return _isRight
                ? right(_right)
                : left(_left);
        }

        public override bool Equals(object obj)
        {
            return obj is Either<TLeft, TLeft> other
                && Equals(_isRight, other._isRight)
                && Equals(_right, other._right)
                && Equals(_left, other._left);
        }

        public override int GetHashCode()
        {
            return _isRight
                ? _right.GetHashCode()
                : _left.GetHashCode();
        }

        public static bool operator ==(Either<TLeft, TRight> lhs, Either<TLeft, TRight> rhs) => lhs.Equals(rhs);

        public static bool operator !=(Either<TLeft, TRight> lhs, Either<TLeft, TRight> rhs) => !lhs.Equals(rhs);
    }
}
