using System.Collections.Immutable;

namespace Funcky.Internal;

internal sealed class PartitionBuilder<TLeft, TRight>
{
    private readonly ImmutableArray<TLeft>.Builder _left = ImmutableArray.CreateBuilder<TLeft>();
    private readonly ImmutableArray<TRight>.Builder _right = ImmutableArray.CreateBuilder<TRight>();

    public PartitionBuilder<TLeft, TRight> AddLeft(TLeft left)
    {
        _left.Add(left);
        return this;
    }

    public PartitionBuilder<TLeft, TRight> AddRight(TRight right)
    {
        _right.Add(right);
        return this;
    }

    public TResult Build<TResult>(Func<IReadOnlyList<TLeft>, IReadOnlyList<TRight>, TResult> selector)
        => selector(_left.ToImmutable(), _right.ToImmutable());
}

internal static class PartitionBuilder
{
    public static Func<PartitionBuilder<TItem, TItem>, TItem, PartitionBuilder<TItem, TItem>> Add<TItem>(Func<TItem, bool> predicate)
        => (builder, item) => predicate(item) ? builder.AddLeft(item) : builder.AddRight(item);

    public static PartitionBuilder<TLeft, TRight> Add<TLeft, TRight>(PartitionBuilder<TLeft, TRight> builder, Either<TLeft, TRight> either)
        where TLeft : notnull
        where TRight : notnull
        => either.Match(left: builder.AddLeft, right: builder.AddRight);

    public static PartitionBuilder<Exception, TValidResult> Add<TValidResult>(PartitionBuilder<Exception, TValidResult> builder, Result<TValidResult> result)
        where TValidResult : notnull
        => result.Match(error: builder.AddLeft, ok: builder.AddRight);
}
