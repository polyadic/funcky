using System;

namespace Funcky
{
    public struct Unit : IEquatable<Unit>, IComparable<Unit>
    {
        public bool Equals(Unit other) => true;

        public int CompareTo(Unit other) => 0;
    }
}
