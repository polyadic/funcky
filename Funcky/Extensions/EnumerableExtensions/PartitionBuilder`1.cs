using System.Collections.Immutable;

namespace Funcky.Extensions;

internal readonly struct PartitionBuilder<TItem>
{
    private readonly Func<TItem, bool> _predicate;

    private readonly IImmutableList<TItem> _left;

    private readonly IImmutableList<TItem> _right;

    public PartitionBuilder(Func<TItem, bool> predicate, IImmutableList<TItem>? left = null, IImmutableList<TItem>? right = null)
    {
        _predicate = predicate;
        _left = left ?? ImmutableList<TItem>.Empty;
        _right = right ?? ImmutableList<TItem>.Empty;
    }

    public static PartitionBuilder<TItem> Add(PartitionBuilder<TItem> builder, TItem item) => builder.Add(item);

    public TResult Build<TResult>(Func<IEnumerable<TItem>, IEnumerable<TItem>, TResult> selector) => selector(_left, _right);

    private PartitionBuilder<TItem> Add(TItem item)
        => _predicate(item)
            ? With(left: _left.Add(item))
            : With(right: _right.Add(item));

    private PartitionBuilder<TItem> With(IImmutableList<TItem>? left = null, IImmutableList<TItem>? right = null)
        => new(_predicate, left ?? _left, right ?? _right);
}
