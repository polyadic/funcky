using System;
using System.Collections.Generic;
using System.Linq;
using Funcky.Monads;

namespace Funcky
{
    public static partial class Sequence
    {
        /// <summary>
        /// Returns an endless <see cref="IEnumerable{T}"/> that yields values based on the previous value
        /// using the <paramref name="next" /> function.
        /// This is essentially the inverse operation of an <see cref="Enumerable.Aggregate{T}"/>.
        /// </summary>
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
        /// Returns an <see cref="IEnumerable{T}"/> that yields values based on the previous value
        /// using the <paramref name="next" /> function. It stops when <paramref name="next" /> returns a
        /// <see cref="Option{TItem}.None"/> value.
        /// This is essentially the inverse operation of an <see cref="Enumerable.Aggregate{T}"/>.
        /// </summary>
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
