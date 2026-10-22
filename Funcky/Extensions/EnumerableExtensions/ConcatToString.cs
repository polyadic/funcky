namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    extension<TSource>(IEnumerable<TSource> source)
    {
        /// <summary>
        /// Concatenates the elements of the given sequence to a single string.
        /// </summary>
        /// <returns>Concatenated string.</returns>
        [Pure]
        public string ConcatToString()
            => string.Concat(source);
    }
}
