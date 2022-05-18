using System.Collections.Immutable;

namespace Funcky.Extensions;

internal sealed class PartitionBuilder<TLeft, TRight>
{
    private ImmutableArray<TLeft>.Builder? _left;
    private ImmutableArray<TRight>.Builder? _right;

    public PartitionBuilder()
    {
        _left = ImmutableArray.CreateBuilder<TLeft>();
        _right = ImmutableArray.CreateBuilder<TRight>();
    }

    private ImmutableArray<TLeft>.Builder Left => _left ??= ImmutableArray.CreateBuilder<TLeft>();

    private ImmutableArray<TRight>.Builder Right => _right ??= ImmutableArray.CreateBuilder<TRight>();

    public PartitionBuilder<TLeft, TRight> AddLeft(TLeft left)
    {
        Left.Add(left);
        return this;
    }

    public PartitionBuilder<TLeft, TRight> AddRight(TRight right)
    {
        Right.Add(right);
        return this;
    }

    public TResult Build<TResult>(Func<IReadOnlyList<TLeft>, IReadOnlyList<TRight>, TResult> selector)
        => selector(
            _left?.ToImmutable() ?? ImmutableArray<TLeft>.Empty,
            _right?.ToImmutable() ?? ImmutableArray<TRight>.Empty);
}

internal static class PartitionBuilder
{
    public static Func<PartitionBuilder<TItem, TItem>, TItem, PartitionBuilder<TItem, TItem>> Add<TItem>(Func<TItem, bool> predicate)
        => (builder, item) => predicate(item) ? builder.AddLeft(item) : builder.AddRight(item);

    public static PartitionBuilder<TLeft, TRight> Add<TLeft, TRight>(PartitionBuilder<TLeft, TRight> builder, Either<TLeft, TRight> either)
        => either.Match(left: builder.AddLeft, right: builder.AddRight);

    public static PartitionBuilder<Exception, TValidResult> Add<TValidResult>(PartitionBuilder<Exception, TValidResult> builder, Result<TValidResult> result)
        => result.Match(error: builder.AddLeft, ok: builder.AddRight);
}
