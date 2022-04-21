namespace Funcky.Monads;

/// <remarks>
/// Either values constructed using <c>default</c> are in an invalid state.
/// Any attempt to perform actions on such a value will throw a <see cref="NotSupportedException"/>.
/// </remarks>
public readonly partial struct Either<TLeft, TRight>
{
    [Pure]
    public Either<TLeft, TResult> Select<TResult>(Func<TRight, TResult> selector)
        => Match(
            left: Either<TLeft, TResult>.Left,
            right: right => selector(right));

    [Pure]
    public Either<TLeft, TResult> SelectMany<TResult>(Func<TRight, Either<TLeft, TResult>> selector)
        => Match(
            left: Either<TLeft, TResult>.Left,
            right: selector);

    [Pure]
    public Either<TLeft, TResult> SelectMany<TEither, TResult>(Func<TRight, Either<TLeft, TEither>> selector, Func<TRight, TEither, TResult> resultSelector)
        => Match(
            left: Either<TLeft, TResult>.Left,
            right: right => selector(right).Select(
                 selectedRight => resultSelector(right, selectedRight)));
}
