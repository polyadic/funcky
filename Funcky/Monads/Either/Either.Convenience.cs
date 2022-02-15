namespace Funcky.Monads;

public readonly partial struct Either<TLeft, TRight>
{
    /// <summary>Performs a side effect when the either is right and returns the either value again.</summary>
    public Either<TLeft, TRight> Inspect(Action<TRight> action)
    {
        Match(left: NoOperation, right: action);
        return this;
    }
}
