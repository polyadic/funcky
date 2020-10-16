using System;
using System.Collections.Generic;
using Funcky.Internal;

namespace Funcky.Monads
{
    /// <summary>
    /// A comparer for two <see cref="Option{TItem}"/>s.
    /// <see cref="Option{T}.None"/> values are always treated as being less than <see cref="Option.Some{T}"/> values.
    /// </summary>
    /// <example>
    /// <code><![CDATA[
    /// using Funcky.Monads;
    /// var withComparer = OptionComparer.Create(new PersonComparer());
    /// var withComparison = OptionComparer<Person>.Create((p1, p2) => p1.Age.CompareTo(p2.Age));
    /// var defaultComparer = OptionComparer<Person>.Default;
    /// ]]></code>
    /// </example>
    public static class OptionComparer<TItem>
        where TItem : notnull
    {
        /// <summary>Returns a default sort order comparer for the item type specified by the generic argument.</summary>
        /// <remarks>See <see cref="Comparer{T}.Default"/>.</remarks>
        public static Comparer<Option<TItem>> Default => OptionComparer.Create(Comparer<TItem>.Default);

        /// <inheritdoc cref="Comparer{T}.Create"/>
        public static Comparer<Option<TItem>> Create(Comparison<TItem> comparison)
            => OptionComparer.Create(Comparer<TItem>.Create(comparison));
    }

    /// <inheritdoc cref="OptionComparer{TItem}"/>
    public static class OptionComparer
    {
        /// <summary>Creates a comparer by using the specified item comparer.</summary>
        /// <param name="comparer">The item comparer to use.</param>
        /// <returns>The new comparer.</returns>
        public static Comparer<Option<TItem>> Create<TItem>(IComparer<TItem> comparer)
            where TItem : notnull
            => new OptionComparerInternal<TItem>(comparer);
    }

    internal sealed class OptionComparerInternal<TItem> : Comparer<Option<TItem>>
        where TItem : notnull
    {
        private readonly IComparer<TItem> _comparer;

        internal OptionComparerInternal(IComparer<TItem> comparer) => _comparer = comparer;

        public override int Compare(Option<TItem> x, Option<TItem> y)
            => (x, y).Match(
                right: _ => ComparisonResult.LessThan,
                none: () => ComparisonResult.Equal,
                left: _ => ComparisonResult.GreaterThan,
                leftAndRight: _comparer.Compare);
    }
}
