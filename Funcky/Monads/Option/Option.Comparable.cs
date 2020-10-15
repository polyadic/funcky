using System;
using System.Collections.Generic;
using Funcky.Internal;

namespace Funcky.Monads
{
    /// <summary>
    /// Comparing Options:
    /// <see cref="None"/> values are always treated as being less than <see cref="Option.Some{T}"/> values.
    /// <see cref="Option.Some{T}"/> values are compared using the type's default comparer.
    /// </summary>
    public readonly partial struct Option<TItem> : IComparable<Option<TItem>>
    {
        public static bool operator <(Option<TItem> lhs, Option<TItem> rhs) => lhs.CompareTo(rhs) < 0;

        public static bool operator >(Option<TItem> lhs, Option<TItem> rhs) => lhs.CompareTo(rhs) > 0;

        public static bool operator <=(Option<TItem> lhs, Option<TItem> rhs) => lhs.CompareTo(rhs) <= 0;

        public static bool operator >=(Option<TItem> lhs, Option<TItem> rhs) => lhs.CompareTo(rhs) >= 0;

        public int CompareTo(Option<TItem> other)
            => (this, other).Match(
                both: Comparer<TItem>.Default.Compare,
                right: _ => ComparisonResult.LessThan,
                neither: () => ComparisonResult.Equal,
                left: _ => ComparisonResult.GreaterThan);
    }
}
