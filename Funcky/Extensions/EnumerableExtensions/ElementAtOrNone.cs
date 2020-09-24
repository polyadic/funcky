using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Funcky.Monads;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// Returns the element at a specified index in a sequence or an Option.None value if the index is out of range.
        /// </summary>
        /// <typeparam name="TSource">The type of element contained by the collection.</typeparam>
        /// <param name="source">The sequence to find an element in.</param>
        /// <param name="index">The index for the element to retrieve.</param>
        /// <returns>The item at the specified index, or Option.None if the index is not found.</returns>
        [Pure]
        public static Option<TSource> ElementAtOrNone<TSource>(this IEnumerable<TSource> source, int index)
            where TSource : notnull
            => source
                .Select(Option.Some)
                .ElementAtOrDefault(index);
    }
}
