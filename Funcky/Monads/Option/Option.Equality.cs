namespace Funcky.Monads;

public readonly partial struct Option<TItem> : IEquatable<Option<TItem>>
{
    public static bool operator ==(Option<TItem> left, Option<TItem> right) => left.Equals(right);

    public static bool operator !=(Option<TItem> left, Option<TItem> right) => !left.Equals(right);

    [Pure]
    public override bool Equals(object? obj)
        => obj is Option<TItem> other && Equals(other);

    [Pure]
    public bool Equals(Option<TItem> other)
        => EqualsWith(other, EqualityComparer<TItem>.Default);

    [Pure]
    public override int GetHashCode()
        => GetHashCodeWith(EqualityComparer<TItem>.Default);

    /// <summary>Equality with a custom item comparer; shared with <see cref="OptionEqualityComparer"/>. Allocates nothing.</summary>
    [Pure]
    internal bool EqualsWith(Option<TItem> other, IEqualityComparer<TItem> comparer)
        => _hasItem
            ? other._hasItem && comparer.Equals(_item, other._item)
            : !other._hasItem;

    [Pure]
    internal int GetHashCodeWith(IEqualityComparer<TItem> comparer)
        => _hasItem
            ? comparer.GetHashCode(_item)
            : 0;
}
