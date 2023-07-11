namespace Funcky.Monads;

public readonly partial struct Either<TLeft, TRight>
{
    public static implicit operator Either<TLeft, TRight>(TRight right) => Right(right);

    /// <summary>Transforms the left value. Returns the original value when it's right.</summary>
    public Either<TResult, TRight> SelectLeft<TResult>(Func<TLeft, TResult> selector)
        where TResult : notnull
        => Match(right: Either<TResult>.Return, left: left => Either<TResult, TRight>.Left(selector(left)));

    /// <summary>Performs a side effect when the either is right and returns the either value again.</summary>
    public Either<TLeft, TRight> Inspect(Action<TRight> inspector)
    {
        Switch(left: NoOperation, right: inspector);
        return this;
    }

    /// <remarks>Careful! This overload discards the left value.</remarks>
    [Pure]
    public Either<TLeft, TRight> OrElse(Either<TLeft, TRight> fallback)
        => Match(left: _ => fallback, right: Either<TLeft>.Return);

    [Pure]
    public Either<TLeft, TRight> OrElse(Func<TLeft, Either<TLeft, TRight>> fallback)
        => Match(left: fallback, right: Either<TLeft>.Return);

    /// <remarks>Careful! This overload discards the left value.</remarks>
    [Pure]
    public TRight GetOrElse(TRight fallback)
        => Match(left: _ => fallback, right: Identity);

    [Pure]
    public TRight GetOrElse(Func<TLeft, TRight> fallback)
        => Match(left: fallback, right: Identity);
}
