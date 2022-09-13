namespace Funcky.Monads;

public static partial class Option
{
    /// <summary>
    /// Creates an <see cref="Option{T}"/> from a nullable value.
    /// </summary>
    [Pure]
    public static Option<TItem> FromNullable<TItem>(TItem? item)
        where TItem : class
        => item is null ? Option<TItem>.None : Some(item);

    /// <summary>
    /// Creates an <see cref="Option{T}"/> from a nullable value.
    /// </summary>
    [Pure]
    public static Option<TItem> FromNullable<TItem>(TItem? item)
        where TItem : struct
        => item.HasValue ? Some(item.Value) : Option<TItem>.None;
}
