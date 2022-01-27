using System.Collections.Immutable;

namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    public static EitherPartitions<TLeft, TRight> Partition<TLeft, TRight>(this IEnumerable<Either<TLeft, TRight>> source)
        => new(ImmutableArray<TLeft>.Empty, ImmutableArray<TRight>.Empty);
}

public readonly struct EitherPartitions<TLeft, TRight>
{
    public EitherPartitions(IReadOnlyCollection<TLeft> left, IReadOnlyCollection<TRight> right) => (Left, Right) = (left, right);

    public IReadOnlyCollection<TLeft> Left { get; }

    public IReadOnlyCollection<TRight> Right { get; }

    public void Deconstruct(out IReadOnlyCollection<TLeft> left, out IReadOnlyCollection<TRight> right) => (left, right) = (Left, Right);
}
