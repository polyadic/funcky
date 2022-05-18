using System.Collections.Immutable;

namespace Funcky.Extensions;

internal readonly struct PartitionBuilder<TLeft, TRight>
{
    public static readonly PartitionBuilder<TLeft, TRight> Default = new(left: null, right: null);

    public PartitionBuilder(IImmutableList<TLeft>? left = null, IImmutableList<TRight>? right = null)
    {
        Left = left ?? ImmutableList<TLeft>.Empty;
        Right = right ?? ImmutableList<TRight>.Empty;
    }

    private IImmutableList<TLeft> Left { get; init; }

    private IImmutableList<TRight> Right { get; init; }

    public PartitionBuilder<TLeft, TRight> AddLeft(TLeft left)
        => this with { Left = Left.Add(left) };

    public PartitionBuilder<TLeft, TRight> AddRight(TRight right)
        => this with { Right = Right.Add(right) };

    public TResult Build<TResult>(Func<IReadOnlyList<TLeft>, IReadOnlyList<TRight>, TResult> selector) => selector(Left, Right);
}

internal static class PartitionBuilder
{
    public static Func<PartitionBuilder<TItem, TItem>, TItem, PartitionBuilder<TItem, TItem>> Add<TItem>(Func<TItem, bool> predicate)
        => (builder, item) => predicate(item) ? builder.AddLeft(item) : builder.AddRight(item);
}
