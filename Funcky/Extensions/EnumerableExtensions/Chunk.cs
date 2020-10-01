using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using static Funcky.Functional;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// Chunks the source sequence into equally sized chunks.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="size">The desired size of the chunks.</param>
        /// <returns>A sequence of equally sized sequences containing elements of the source collection in the same order.</returns>
        [Pure]
        public static IEnumerable<IEnumerable<TSource>> Chunk<TSource>(this IEnumerable<TSource> source, int size)
            => Chunk(source, size, Identity);

        /// <summary>
        /// Chunks the source sequence into equally sized chunks.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
        /// <typeparam name="TResult">Type of the elements returned.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="size">The desired size of the chunks.</param>
        /// <param name="resultSelector">The result selector will be applied on each chunked sequence and can produce a desired result.</param>
        /// <returns>A sequence of results based on equally sized chunks.</returns>
        [Pure]
        public static IEnumerable<TResult> Chunk<TSource, TResult>(this IEnumerable<TSource> source, int size, Func<IEnumerable<TSource>, TResult> resultSelector)
        {
            ValidateChunkSize(size);

            return ChunkEnumerable(source, size, resultSelector);
        }

        private static void ValidateChunkSize(int size)
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
                yield return resultSelector(TakeSkip(sourceEnumerator, size).ToImmutableList());
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
