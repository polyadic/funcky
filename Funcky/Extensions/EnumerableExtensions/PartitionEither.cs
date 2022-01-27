using System.Collections.Immutable;

namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    public static EitherPartitions<TLeft, TRight> Partition<TLeft, TRight>(this IEnumerable<Either<TLeft, TRight>> source)
        => source
            .Aggregate(
                new PartitionBuilder<TLeft, TRight>(left: null, right: null),
                (builder, either) => either.Match(left: builder.AddLeft, right: builder.AddRight))
            .Build((left, right) => new EitherPartitions<TLeft, TRight>(left, right));

    private readonly struct PartitionBuilder<TLeft, TRight>
    {
        private readonly IImmutableList<TLeft> _left;

        private readonly IImmutableList<TRight> _right;

        public PartitionBuilder(IImmutableList<TLeft>? left = null, IImmutableList<TRight>? right = null)
        {
            _left = left ?? ImmutableList<TLeft>.Empty;
            _right = right ?? ImmutableList<TRight>.Empty;
        }

        public PartitionBuilder<TLeft, TRight> AddLeft(TLeft left)
            => With(left: _left.Add(left));

        public PartitionBuilder<TLeft, TRight> AddRight(TRight right)
            => With(right: _right.Add(right));

        public TResult Build<TResult>(Func<IReadOnlyCollection<TLeft>, IReadOnlyCollection<TRight>, TResult> selector) => selector(_left, _right);

        public PartitionBuilder<TLeft, TRight> With(IImmutableList<TLeft>? left = null, IImmutableList<TRight>? right = null) => new(left ?? _left, right ?? _right);
    }
}

public readonly struct EitherPartitions<TLeft, TRight>
{
    public EitherPartitions(IReadOnlyCollection<TLeft> left, IReadOnlyCollection<TRight> right) => (Left, Right) = (left, right);

    public IReadOnlyCollection<TLeft> Left { get; }

    public IReadOnlyCollection<TRight> Right { get; }

    public void Deconstruct(out IReadOnlyCollection<TLeft> left, out IReadOnlyCollection<TRight> right) => (left, right) = (Left, Right);
}
