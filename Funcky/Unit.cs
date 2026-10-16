namespace Funcky;

public readonly struct Unit : IEquatable<Unit>, IComparable<Unit>, IComparable
{
    [Pure]
    public static Unit Value => default;

    [Pure]
    public static bool operator ==(Unit left, Unit right) => true;

    [Pure]
    public static bool operator !=(Unit left, Unit right) => false;

    [Pure]
    public static bool operator <(Unit left, Unit right) => false;

    [Pure]
    public static bool operator <=(Unit left, Unit right) => true;

    [Pure]
    public static bool operator >(Unit left, Unit right) => false;

    [Pure]
    public static bool operator >=(Unit left, Unit right) => true;

    [Pure]
    public bool Equals(Unit other) => true;

    [Pure]
    public override bool Equals(object? obj) => obj is Unit;

    [Pure]
    public override int GetHashCode() => 0;

    [Pure]
    public int CompareTo(Unit other) => 0;

    [Pure]
    public int CompareTo(object? obj)
        => obj switch
        {
            null => 1,
            Unit => 0,
            _ => throw new ArgumentException("Object must be of type Unit.", nameof(obj)),
        };

    [Pure]
    public override string ToString() => "Unit";
}
