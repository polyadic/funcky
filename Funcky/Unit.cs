#nullable enable

using System;

namespace Funcky
{
    public readonly struct Unit : IEquatable<Unit>, IComparable<Unit>
    {
        public static bool operator ==(Unit lhs, Unit rhs) => true;

        public static bool operator !=(Unit lhs, Unit rhs) => false;

        public bool Equals(Unit other) => true;

        public override bool Equals(object obj) => obj is Unit other && Equals(other);

        public override int GetHashCode() => 0;

        public int CompareTo(Unit other) => 0;
    }
}
