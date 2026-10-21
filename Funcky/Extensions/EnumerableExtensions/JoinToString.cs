namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    /// <param name="source">A sequence of items to be joined in a string.</param>
    /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    extension<TSource>(IEnumerable<TSource> source)
    {
        /// <summary>
        /// Concatenates the elements of the given sequence, using the specified separator between each element or member.
        /// </summary>
        /// <param name="separator">A single character to separate the individual elements.</param>
        /// <returns>Joined string with separators between the elements.</returns>
        [Pure]
        public string JoinToString(char separator)
#if JOIN_TO_STRING_CHAR_SEPARATOR
            => string.Join(separator, source);
#else
            => string.Join(separator.ToString(), source);
#endif

        /// <summary>
        /// Concatenates the elements of the given sequence, using the specified separator between each element or member.
        /// </summary>
        /// <param name="separator">A string to separate the individual elements.</param>
        /// <returns>Joined string with separators between the elements.</returns>
        [Pure]
        public string JoinToString(string separator)
            => string.Join(separator, source);
    }

    /// <param name="source">A sequence of items to be joined in a string.</param>
    extension(IEnumerable<string?> source)
    {
        /// <summary>
        /// Concatenates the elements of the given sequence, using the specified separator between each element or member.
        /// </summary>
        /// <param name="separator">A string to separate the individual elements.</param>
        /// <returns>Joined string with separators between the elements.</returns>
        [Pure]
        public string JoinToString(string separator)
            => string.Join(separator, source);
    }
}
