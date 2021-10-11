using System.Diagnostics.Contracts;
using Funcky.Extensions;

namespace Funcky.Async.Extensions
{
    public static partial class AsyncEnumerableExtensions
    {
        /// <summary>
        /// Returns a sequence mapping each element into a type which has an IsFirst property which is true for the first element of the sequence. The returned struct is deconstructible.
        /// </summary>
        /// <param name="source">The source sequence.</param>
        /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
        /// <returns>Returns a sequence mapping each element into a type which has an IsFirst property which is true for the first element of the sequence.</returns>
        [Pure]
        public static async IAsyncEnumerable<ValueWithFirst<TSource>> WithFirst<TSource>(this IAsyncEnumerable<TSource> source)
        {
            await using var enumerator = source.GetAsyncEnumerator();

            if (!await enumerator.MoveNextAsync())
            {
                yield break;
            }

            yield return new ValueWithFirst<TSource>(enumerator.Current, true);

            while (await enumerator.MoveNextAsync())
            {
                yield return new ValueWithFirst<TSource>(enumerator.Current, false);
            }
        }
    }
}
