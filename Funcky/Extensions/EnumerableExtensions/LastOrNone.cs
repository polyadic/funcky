namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    /// <typeparam name="TSource">the inner type of the enumerable.</typeparam>
    extension<TSource>(IEnumerable<TSource> source)
        where TSource : notnull
    {
        /// <summary>
        /// Returns the last element of a sequence as an <see cref="Option{T}" />, or a <see cref="Option{T}.None" /> value if the sequence contains no elements.
        /// </summary>
        [Pure]
        public Option<TSource> LastOrNone()
            => source
                .Select(Option.Some)
                .LastOrDefault();

        /// <summary>
        /// Returns the last element of a sequence that satisfies a condition as an <see cref="Option{T}" />  or a <see cref="Option{T}.None" /> value if no such element is found.
        /// </summary>
        [Pure]
        public Option<TSource> LastOrNone(Func<TSource, bool> predicate)
            => source
                .Where(predicate)
                .Select(Option.Some)
                .LastOrDefault();
    }
}
