namespace Funcky;

public readonly struct Unit : IEquatable<Unit>, IComparable<Unit>
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
}
