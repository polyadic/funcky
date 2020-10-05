using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Funcky.GenericConstraints;
using Funcky.Monads;

namespace Funcky
{
    public static class Sequence
    {
        public static IEnumerable<TItem> Return<TItem>(TItem item) => Enumerable.Repeat(item, 1);

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

        /// <returns>An <see cref="IEnumerable{T}" /> consisting of a single item or zero items.</returns>
        [Pure]
        public static IEnumerable<T> FromNullable<T>(T? item, RequireClass<T>? ω = null)
            where T : class
            => item is { } ? Return(item) : Enumerable.Empty<T>();

        /// <inheritdoc cref="FromNullable{T}(T, Funcky.GenericConstraints.RequireClass{T})"/>
        [Pure]
        public static IEnumerable<T> FromNullable<T>(T? item, RequireStruct<T>? ω = null)
            where T : struct
            => item.HasValue ? Return(item.Value) : Enumerable.Empty<T>();
    }
}
