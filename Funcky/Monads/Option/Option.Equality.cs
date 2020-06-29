using System;
using System.Diagnostics.Contracts;

namespace Funcky.Monads
{
    public readonly partial struct Option<TItem> : IEquatable<Option<TItem>>
    {
        [Pure]
        public static bool operator ==(Option<TItem> lhs, Option<TItem> rhs) => lhs.Equals(rhs);

        [Pure]
        public static bool operator !=(Option<TItem> lhs, Option<TItem> rhs) => !lhs.Equals(rhs);

        [Pure]
        public override bool Equals(object obj)
            => obj is Option<TItem> other && Equals(other);

        [Pure]
        public bool Equals(Option<TItem> other)
            => Equals(_hasItem, other._hasItem) && Equals(_item, other._item);

        [Pure]
        public override int GetHashCode()
            => Match(
                none: 0,
                some: item => item.GetHashCode());
    }
}
