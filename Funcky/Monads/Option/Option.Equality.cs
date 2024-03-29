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
        => OptionEqualityComparer<TItem>.Default.Equals(this, other);

    [Pure]
    public override int GetHashCode()
        => OptionEqualityComparer<TItem>.Default.GetHashCode(this);
}
