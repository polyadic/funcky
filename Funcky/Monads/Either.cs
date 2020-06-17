using System;

namespace Funcky.Monads
{
    public readonly struct Either<TLeft, TRight>
    {
        private readonly TLeft _left;
        private readonly TRight _right;
        private readonly Side _side;

        private Either(TLeft left)
        {
            _left = left;
            _right = default!;
            _side = Side.Left;
        }

        private Either(TRight right)
        {
            _left = default!;
            _right = right;
            _side = Side.Right;
        }

        private enum Side : byte
        {
            Uninitialized,
            Left,
            Right,
        }

        public static bool operator ==(Either<TLeft, TRight> lhs, Either<TLeft, TRight> rhs) => lhs.Equals(rhs);

        public static bool operator !=(Either<TLeft, TRight> lhs, Either<TLeft, TRight> rhs) => !lhs.Equals(rhs);

        public static Either<TLeft, TRight> Left(TLeft left) => new Either<TLeft, TRight>(left);

        public static Either<TLeft, TRight> Right(TRight right) => new Either<TLeft, TRight>(right);

        public Either<TLeft, TResult> Select<TResult>(Func<TRight, TResult> selector)
            => Match(
                Either<TLeft, TResult>.Left,
                right => Either<TLeft, TResult>.Right(selector(right)));

        public Either<TLeft, TResult> SelectMany<TEither, TResult>(Func<TRight, Either<TLeft, TEither>> eitherSelector, Func<TRight, TEither, TResult> resultSelector)
            => Match(
                Either<TLeft, TResult>.Left,
                right => eitherSelector(right).Select(
                    selectedRight => resultSelector(right, selectedRight)));

        public TMatchResult Match<TMatchResult>(Func<TLeft, TMatchResult> left, Func<TRight, TMatchResult> right)
            => _side switch
            {
                Side.Left => left(_left),
                Side.Right => right(_right),
                Side.Uninitialized => throw new NotSupportedException(
                    "Either constructed via default instead of a factory function (Either.Left or Either.Right)"),
                _ => throw new NotSupportedException(
                    $"Internal error: Enum variant {_side} is not handled"),
            };

        public override bool Equals(object obj)
            => obj is Either<TLeft, TLeft> other
               && Equals(_side, other._side)
               && Equals(_right, other._right)
               && Equals(_left, other._left);

        public override int GetHashCode()
            => Match(
                left => left?.GetHashCode(),
                right => right?.GetHashCode()) ?? 0;
    }
}
