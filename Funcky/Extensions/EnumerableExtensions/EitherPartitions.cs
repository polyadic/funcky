namespace Funcky.Extensions;

public readonly struct EitherPartitions<TLeft, TRight>
    where TLeft : notnull
    where TRight : notnull
{
    public EitherPartitions(IReadOnlyList<TLeft> left, IReadOnlyList<TRight> right) => (Left, Right) = (left, right);

    public IReadOnlyList<TLeft> Left { get; }

    public IReadOnlyList<TRight> Right { get; }

    public void Deconstruct(out IReadOnlyList<TLeft> left, out IReadOnlyList<TRight> right) => (left, right) = (Left, Right);
}

public static class EitherPartitions
{
    public static EitherPartitions<TLeft, TRight> Create<TLeft, TRight>(IReadOnlyList<TLeft> left, IReadOnlyList<TRight> right)
        where TLeft : notnull
        where TRight : notnull
        => new(left, right);
}
