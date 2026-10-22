namespace Funcky.Monads;

public static partial class EitherExtensions
{
    extension<TLeft, TRight>(Either<TLeft, Either<TLeft, TRight>> either)
        where TLeft : notnull
        where TRight : notnull
    {
        [Pure]
        public Either<TLeft, TRight> Flatten()
#pragma warning disable λ1018 // This is the implementation of Flatten.
            => either.SelectMany(Identity);
    }

#pragma warning restore λ1018
    extension<TLeft, TRight>(Either<TLeft, TRight> either)
        where TLeft : notnull
        where TRight : notnull
    {
        /// <summary>Returns the left value or <see cref="Option{TItem}.None()"/> if the <c>either</c> is a right value.</summary>
        [Pure]
        public Option<TLeft> LeftOrNone()
            => either.Match(
                left: Option.Some,
                right: static _ => Option<TLeft>.None);

        /// <summary>Returns the right value or <see cref="Option{TItem}.None()"/> if the <c>either</c> is a left value.</summary>
        [Pure]
        public Option<TRight> RightOrNone()
            => either.Match(
                left: static _ => Option<TRight>.None,
                right: Option.Some);
    }
}
