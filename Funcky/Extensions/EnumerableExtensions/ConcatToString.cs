namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// Concatenates the elements of the given sequence to a single string.
        /// </summary>
        /// <typeparam name="T">Type of the elements in <paramref name="source"/> sequence.</typeparam>
        /// <returns>Concatenated string.</returns>
        [Pure]
        public static string ConcatToString<T>(this IEnumerable<T> source)
            => string.Concat(source);
    }
}
