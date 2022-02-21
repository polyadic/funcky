namespace Funcky.Extensions;

public readonly struct EitherPartitions<TLeft, TRight>
{
    public EitherPartitions(IReadOnlyCollection<TLeft> left, IReadOnlyCollection<TRight> right) => (Left, Right) = (left, right);

    public IReadOnlyCollection<TLeft> Left { get; }

    public IReadOnlyCollection<TRight> Right { get; }

    public void Deconstruct(out IReadOnlyCollection<TLeft> left, out IReadOnlyCollection<TRight> right) => (left, right) = (Left, Right);
}
