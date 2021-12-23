using System.Collections.Immutable;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// Chunks the source sequence into equally sized chunks. The last chunk can be smaller.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="size">The desired size of the chunks.</param>
        /// <returns>A sequence of equally sized sequences containing elements of the source collection in the same order.</returns>
        [Pure]
#if NET6_0_OR_GREATER
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static IEnumerable<IEnumerable<TSource>> Chunk<TSource>(IEnumerable<TSource> source, int size)
#else
        public static IEnumerable<IEnumerable<TSource>> Chunk<TSource>(this IEnumerable<TSource> source, int size)
#endif
            => ChunkEnumerable(source, ValidateChunkSize(size));

        /// <summary>
        /// Chunks the source sequence into equally sized chunks. The last chunk can be smaller.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
        /// <typeparam name="TResult">Type of the elements returned.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="size">The desired size of the chunks.</param>
        /// <param name="resultSelector">The result selector will be applied on each chunked sequence and can produce a desired result.</param>
        /// <returns>A sequence of results based on equally sized chunks.</returns>
        [Pure]
        public static IEnumerable<TResult> Chunk<TSource, TResult>(this IEnumerable<TSource> source, int size, Func<IEnumerable<TSource>, TResult> resultSelector)
            => ChunkEnumerable(source, ValidateChunkSize(size))
                .Select(resultSelector);

        private static int ValidateChunkSize(int size)
            => size > 0
                ? size
                : throw new ArgumentOutOfRangeException(nameof(size), size, "Size must be bigger than 0");

        private static IEnumerable<IEnumerable<TSource>> ChunkEnumerable<TSource>(IEnumerable<TSource> source, int size)
        {
            using var sourceEnumerator = source.GetEnumerator();

            while (sourceEnumerator.MoveNext())
            {
                yield return TakeSkip(sourceEnumerator, size).ToImmutableList();
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
