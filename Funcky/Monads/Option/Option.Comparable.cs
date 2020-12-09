using System;
using System.Collections;

namespace Funcky.Monads
{
    /// <summary>
    /// Comparing Options:
    /// <see cref="None"/> values are always treated as being less than <see cref="Option.Some{T}"/> values.
    /// <see cref="Option.Some{T}"/> values are compared using the type's default comparer.
    /// </summary>
    public readonly partial struct Option<TItem> : IComparable<Option<TItem>>, IComparable
    {
        public static bool operator <(Option<TItem> lhs, Option<TItem> rhs) => lhs.CompareTo(rhs) < 0;

        public static bool operator >(Option<TItem> lhs, Option<TItem> rhs) => lhs.CompareTo(rhs) > 0;

        public static bool operator <=(Option<TItem> lhs, Option<TItem> rhs) => lhs.CompareTo(rhs) <= 0;

        public static bool operator >=(Option<TItem> lhs, Option<TItem> rhs) => lhs.CompareTo(rhs) >= 0;

        public int CompareTo(object? obj)
            => (OptionComparer<TItem>.Default as IComparer).Compare(this, obj);

        /// <exception cref="T:System.ArgumentException">Thrown when two <see cref="Option.Some{T}"/> values are compared
        /// and the type <typeparamref name="TItem"/> does not implement either the <see cref="IComparable{T}" />
        /// generic interface or the <see cref="IComparable" /> interface.</exception>
        public int CompareTo(Option<TItem> other)
            => OptionComparer<TItem>.Default.Compare(this, other);
    }
}
