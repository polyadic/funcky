namespace Funcky.Monads;

public static partial class EitherExtensions
{
    public static Either<TLeft, TRight> Flatten<TLeft, TRight>(this Either<TLeft, Either<TLeft, TRight>> either)
        where TLeft : notnull
        where TRight : notnull
        => either.SelectMany(Identity);

    /// <summary>Returns the left value or <see cref="Option{TItem}.None()"/> if the <paramref name="either"/> is a right value.</summary>
    [Pure]
    public static Option<TLeft> LeftOrNone<TLeft, TRight>(this Either<TLeft, TRight> either)
        where TLeft : notnull
        where TRight : notnull
        => either.Match(
            left: Option.Some,
            right: static _ => Option<TLeft>.None);

    /// <summary>Returns the right value or <see cref="Option{TItem}.None()"/> if the <paramref name="either"/> is a left value.</summary>
    [Pure]
    public static Option<TRight> RightOrNone<TLeft, TRight>(this Either<TLeft, TRight> either)
        where TLeft : notnull
        where TRight : notnull
        => either.Match(
            left: static _ => Option<TRight>.None,
            right: Option.Some);
}
