using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Funcky.Monads;

namespace Funcky
{
    public static partial class Sequence
    {
        /// <summary>
        /// Returns an endless <see cref="IEnumerable{T}"/> that yields values using a <paramref name="next"/> function.
        /// This is essentially the inverse operation of an <see cref="Enumerable.Aggregate{T}"/>.
        /// </summary>
        /// <param name="seed">The first value passed to <paramref name="next"/>. Not included in the returned <see cref="IEnumerable{T}"/>.</param>
        /// <param name="next">Generates the next item based on the previous item.</param>
        [Pure]
        public static IEnumerable<TItem> Generate<TItem>(TItem seed, Func<TItem, TItem> next)
            where TItem : notnull
        {
            var previousItem = seed;
            while (true)
            {
                yield return previousItem = next(previousItem);
            }
        }

        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"/> that yields values using the <paramref name="next"/> function
        /// until a <see cref="Option{TItem}.None"/> is returned.
        /// This is essentially the inverse operation of an <see cref="Enumerable.Aggregate{T}"/>.
        /// </summary>
        /// <param name="seed">The first value passed to <paramref name="next"/>. Not included in the returned <see cref="IEnumerable{T}"/>.</param>
        /// <param name="next">Generates the next item or <see cref="Option{TItem}.None"/> based on the previous item.</param>
        [Pure]
        public static IEnumerable<TItem> Generate<TItem>(TItem seed, Func<TItem, Option<TItem>> next)
            where TItem : notnull
        {
            var previousItem = seed;
            while (next(previousItem).TryGetValue(out var item))
            {
                yield return previousItem = item;
            }
        }
    }
}
