using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// Chunks the source sequence into sized chunks.
        /// </summary>
        /// <typeparam name="TSource">Type of elements in <paramref name="source"/> sequence.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="size">Size of chunks.</param>
        /// <returns>A sequence of equally sized chunks containing elements of the source collection.</returns>
        [Pure]
        public static IEnumerable<IEnumerable<TSource>> Chunk<TSource>(this IEnumerable<TSource> source, int size)
            => Chunk(source, size, x => x);

        [Pure]
        public static IEnumerable<TResult> Chunk<TSource, TResult>(this IEnumerable<TSource> source, int size, Func<IEnumerable<TSource>, TResult> resultSelector)
        {
            while (source.Any())
            {
                yield return resultSelector(source.Take(size));
                source = source.Skip(size);
            }
        }
    }
}
