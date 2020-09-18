using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using static Funcky.Functional;

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
            => Chunk(source, size, Identity);

        [Pure]
        public static IEnumerable<TResult> Chunk<TSource, TResult>(this IEnumerable<TSource> source, int size, Func<IEnumerable<TSource>, TResult> resultSelector)
        {
            ValidateSize(size);

            return ChunkEnumerable(source, size, resultSelector);
        }

        private static void ValidateSize(int size)
        {
            if (size <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(size), size, "Size must be bigger than 0");
            }
        }

        private static IEnumerable<TResult> ChunkEnumerable<TSource, TResult>(IEnumerable<TSource> source, int size, Func<IEnumerable<TSource>, TResult> resultSelector)
        {
            using var sourceEnumerator = source.GetEnumerator();

            while (sourceEnumerator.MoveNext())
            {
                yield return resultSelector(TakeSkip(sourceEnumerator, size).ToList());
            }
        }

        private static IEnumerable<TSource> TakeSkip<TSource>(IEnumerator<TSource> source, int size)
        {
            do
            {
                yield return source.Current;
            }
            while (--size > 0 && source.MoveNext());
        }
    }
}
