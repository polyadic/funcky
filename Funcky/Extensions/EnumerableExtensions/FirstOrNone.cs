namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    /// <typeparam name="TSource">the inner type of the enumerable.</typeparam>
    extension<TSource>(IEnumerable<TSource> source)
        where TSource : notnull
    {
        /// <summary>
        /// Returns the first element of a sequence as an <see cref="Option{T}" />, or a <see cref="Option{T}.None" /> value if the sequence contains no elements.
        /// </summary>
        [Pure]
        public Option<TSource> FirstOrNone()
            => source
                .Select(Option.Some)
                .FirstOrDefault();

        /// <summary>
        /// Returns the first element of the sequence as an <see cref="Option{T}" /> that satisfies a condition or a <see cref="Option{T}.None" /> value if no such element is found.
        /// </summary>
        [Pure]
        public Option<TSource> FirstOrNone(Func<TSource, bool> predicate)
            => source
                .Where(predicate)
                .Select(Option.Some)
                .FirstOrDefault();
    }
}
