using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// Returns a sequence mapping each element together with an Index starting at 0. The returned struct is deconstructible.
        /// </summary>
        /// <param name="source">The source sequence.</param>
        /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
        /// <returns>Returns a sequence mapping each element together with an Index starting at 0.</returns>
        [Pure]
        public static IEnumerable<ValueWithIndex<TSource>> WithIndex<TSource>(this IEnumerable<TSource> source)
            => source.Select((value, index) => new ValueWithIndex<TSource>(value, index));
    }
}
