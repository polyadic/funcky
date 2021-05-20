using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Funcky.Monads;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>Returns a sequence mapping each element together with its predecessor.</summary>
        /// <exception cref="ArgumentNullException">Thrown when any value in <paramref name="source"/> is <see langword="null"/>.</exception>
        [Pure]
        public static IEnumerable<ValueWithPrevious<TSource>> WithPrevious<TSource>(this IEnumerable<TSource> source)
            where TSource : notnull
        {
            var previous = Option<TSource>.None;

            foreach (var value in source)
            {
                yield return new ValueWithPrevious<TSource>(value, previous);
                previous = value;
            }
        }
    }
}
