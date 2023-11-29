using Funcky.Internal;

namespace Funcky.Monads;

/// <summary>
/// An equality comparer for two <see cref="Option{TItem}"/>s.
/// <example>
/// <code><![CDATA[
/// using Funcky.Monads;
/// var customComparer = OptionEqualityComparer.Create(new PersonComparer());
/// var defaultComparer = OptionEqualityComparer<Person>.Default;
/// ]]></code>
/// </example>
/// </summary>
public static class OptionEqualityComparer<TItem>
    where TItem : notnull
{
    /// <summary>Returns a default sort order comparer for the item type specified by the generic argument.</summary>
    /// <seealso cref="EqualityComparer{T}.Default"/>
    public static EqualityComparer<Option<TItem>> Default => OptionEqualityComparer.Create(EqualityComparer<TItem>.Default);
}

/// <inheritdoc cref="OptionEqualityComparer{TItem}"/>
public static class OptionEqualityComparer
{
    /// <summary>Creates a comparer by using the specified item comparer.</summary>
    /// <param name="comparer">The item comparer to use.</param>
    /// <returns>The new comparer.</returns>
    public static EqualityComparer<Option<TItem>> Create<TItem>(IEqualityComparer<TItem> comparer)
        where TItem : notnull
        => new OptionEqualityComparerInternal<TItem>(comparer);
}

internal sealed class OptionEqualityComparerInternal<TItem>(IEqualityComparer<TItem> comparer) : EqualityComparer<Option<TItem>>
    where TItem : notnull
{
    public override bool Equals(Option<TItem> x, Option<TItem> y)
        => (x, y).Match(
            right: False,
            none: True,
            left: False,
            leftAndRight: comparer.Equals);

    public override int GetHashCode(Option<TItem> option)
        => option.Match(
            none: 0,
            some: comparer.GetHashCode);
}
