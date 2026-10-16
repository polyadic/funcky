using System.Collections;
using Funcky.Internal;

namespace Funcky.Monads;

/// <summary>
/// Comparing Options:
/// <see cref="None"/> values are always treated as being less than <see cref="Option.Some{T}"/> values.
/// <see cref="Option.Some{T}"/> values are compared using the type's default comparer.
/// </summary>
public readonly partial struct Option<TItem> : IComparable<Option<TItem>>, IComparable
{
    public static bool operator <(Option<TItem> left, Option<TItem> right) => left.CompareTo(right) < 0;

    public static bool operator >(Option<TItem> left, Option<TItem> right) => left.CompareTo(right) > 0;

    public static bool operator <=(Option<TItem> left, Option<TItem> right) => left.CompareTo(right) <= 0;

    public static bool operator >=(Option<TItem> left, Option<TItem> right) => left.CompareTo(right) >= 0;

    public int CompareTo(object? obj)
        => (OptionComparer<TItem>.Default as IComparer).Compare(this, obj);

    /// <exception cref="T:System.ArgumentException">Thrown when two <see cref="Option.Some{T}"/> values are compared
    /// and the type <typeparamref name="TItem"/> does not implement either the <see cref="IComparable{T}" />
    /// generic interface or the <see cref="IComparable" /> interface.</exception>
    public int CompareTo(Option<TItem> other)
        => CompareWith(other, Comparer<TItem>.Default);

    /// <summary>Comparison with a custom item comparer; shared with <see cref="OptionComparer"/>. Allocates nothing.</summary>
    [Pure]
    internal int CompareWith(Option<TItem> other, IComparer<TItem> comparer)
        => (_hasItem, other._hasItem) switch
        {
            (true, true) => comparer.Compare(_item, other._item),
            (true, false) => ComparisonResult.GreaterThan,
            (false, true) => ComparisonResult.LessThan,
            (false, false) => ComparisonResult.Equal,
        };
}
