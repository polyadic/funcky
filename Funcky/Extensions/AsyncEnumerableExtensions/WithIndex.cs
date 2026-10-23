#if ASYNC_SUPPORTED
namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <param name="source">The source sequence.</param>
    /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    extension<TSource>(IAsyncEnumerable<TSource> source)
    {
        /// <summary>
        /// Returns a sequence mapping each element together with an Index starting at 0. The returned struct is deconstructible.
        /// </summary>
        /// <returns>Returns a sequence mapping each element together with an Index starting at 0.</returns>
        [Pure]
        public IAsyncEnumerable<ValueWithIndex<TSource>> WithIndex()
            => source.Select((value, index) => new ValueWithIndex<TSource>(value, index));
    }
}
#endif
