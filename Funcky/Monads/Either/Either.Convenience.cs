namespace Funcky.Monads;

public readonly partial struct Either<TLeft, TRight>
{
    public static implicit operator Either<TLeft, TRight>(TRight right) => Right(right);

    /// <summary>Performs a side effect when the either is right and returns the either value again.</summary>
    public Either<TLeft, TRight> Inspect(Action<TRight> action)
    {
        Switch(left: NoOperation, right: action);
        return this;
    }
}
