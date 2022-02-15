using System.Collections.Immutable;

namespace Funcky.Extensions;

internal readonly struct PartitionBuilder<TLeft, TRight>
{
    public static readonly PartitionBuilder<TLeft, TRight> Default = new(left: null, right: null);

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
