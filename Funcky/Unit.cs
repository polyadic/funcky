using System.Diagnostics.Contracts;

namespace Funcky
{
    public readonly struct Unit : IEquatable<Unit>, IComparable<Unit>
    {
        [Pure]
        public static Unit Value => default;

        [Pure]
        public static bool operator ==(Unit lhs, Unit rhs) => true;

        [Pure]
        public static bool operator !=(Unit lhs, Unit rhs) => false;

        [Pure]
        public static bool operator <(Unit lhs, Unit rhs) => false;

        [Pure]
        public static bool operator <=(Unit lhs, Unit rhs) => true;

        [Pure]
        public static bool operator >(Unit lhs, Unit rhs) => false;

        [Pure]
        public static bool operator >=(Unit lhs, Unit rhs) => true;

        [Pure]
        public bool Equals(Unit other) => true;

        [Pure]
        public override bool Equals(object? obj) => obj is Unit other && Equals(other);

        [Pure]
        public override int GetHashCode() => 0;

        [Pure]
        public int CompareTo(Unit other) => 0;
    }
}
