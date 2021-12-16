namespace Funcky.Monads
{
    /// <remarks>
    /// Either values constructed using <c>default</c> are in an invalid state.
    /// Any attempt to perform actions on such a value will throw a <see cref="NotSupportedException"/>.
    /// </remarks>
    public readonly partial struct Either<TLeft, TRight> : IEquatable<Either<TLeft, TRight>>
    {
        private const string UninitializedMatch = "Either constructed via default instead of a factory function (Either.Left or Either.Right)";
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

        private string UnknownSide => $"Internal error: Enum variant {_side} is not handled";

        [Pure]
        public static bool operator ==(Either<TLeft, TRight> lhs, Either<TLeft, TRight> rhs) => lhs.Equals(rhs);

        [Pure]
        public static bool operator !=(Either<TLeft, TRight> lhs, Either<TLeft, TRight> rhs) => !lhs.Equals(rhs);

        [Pure]
        public static Either<TLeft, TRight> Left(TLeft left) => new(left);

        [Pure]
        public static Either<TLeft, TRight> Right(TRight right) => new(right);

        [Pure]
        public TMatchResult Match<TMatchResult>(Func<TLeft, TMatchResult> left, Func<TRight, TMatchResult> right)
            => _side switch
            {
                Side.Left => left(_left),
                Side.Right => right(_right),
                Side.Uninitialized => throw new NotSupportedException(UninitializedMatch),
                _ => throw new NotSupportedException(UnknownSide),
            };

        public void Match(Action<TLeft> left, Action<TRight> right)
        {
            switch (_side)
            {
                case Side.Left:
                    left(_left);
                    break;
                case Side.Right:
                    right(_right);
                    break;
                case Side.Uninitialized:
                    throw new NotSupportedException(UninitializedMatch);
                default:
                    throw new NotSupportedException(UnknownSide);
            }
        }

        [Pure]
        public override bool Equals(object? obj)
            => obj is Either<TLeft, TRight> other && Equals(other);

        [Pure]
        public bool Equals(Either<TLeft, TRight> other)
            => Equals(_side, other._side)
               && Equals(_right, other._right)
               && Equals(_left, other._left);

        [Pure]
        public override int GetHashCode()
            => Match(
                left => left?.GetHashCode(),
                right => right?.GetHashCode()) ?? 0;

        [Pure]
        public Either<TRight, TLeft> Flip()
            => Match(
                left: Either<TRight, TLeft>.Right,
                right: Either<TRight, TLeft>.Left);
    }

    public static class Either<TLeft>
    {
        [Pure]
        public static Either<TLeft, TRight> Return<TRight>(TRight item)
            => Either<TLeft, TRight>.Right(item);
    }
}
