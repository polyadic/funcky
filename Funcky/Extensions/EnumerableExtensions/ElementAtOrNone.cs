namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    /// <param name="source">The sequence to find an element in.</param>
    /// <typeparam name="TSource">The type of element contained by the sequence.</typeparam>
    extension<TSource>(IEnumerable<TSource> source)
        where TSource : notnull
    {
        /// <summary>
        /// Returns the element at a specified index in a sequence or an <see cref="Option{T}.None" /> value if the index is out of range.
        /// </summary>
        /// <param name="index">The index for the element to retrieve.</param>
        /// <returns>The item at the specified index, or <see cref="Option{T}.None" /> if the index is not found.</returns>
        [Pure]
        public Option<TSource> ElementAtOrNone(int index)
            => source
                .Select(Option.Some)
                .ElementAtOrDefault(index);
    }

#if ELEMENT_AT_INDEX
    /// <param name="source">The sequence to find an element in.</param>
    /// <typeparam name="TSource">The type of element contained by the sequence.</typeparam>
    extension<TSource>(IEnumerable<TSource> source)
        where TSource : notnull
    {
        /// <summary>
        /// Returns the element at a specified index in a sequence or an <see cref="Option{T}.None" /> value if the index is out of range.
        /// </summary>
        /// <param name="index">The index for the element to retrieve.</param>
        /// <returns>The item at the specified index, or <see cref="Option{T}.None" /> if the index is not found.</returns>
        [Pure]
        public Option<TSource> ElementAtOrNone(Index index)
            => source
                .Select(Option.Some)
                .ElementAtOrDefault(index);
    }
#endif
}
