using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// Concatenates the elements of the given sequence, using the specified separator between each element or member.
        /// </summary>
        /// <typeparam name="T">Type of the elements in <paramref name="source"/> sequence.</typeparam>
        /// <param name="source">A sequence of items to be joined in a string.</param>
        /// <param name="separator">A single character to separate the invidual elements.</param>
        /// <returns>Joined string with separators between the elements.</returns>
        [Pure]
        public static string JoinToString<T>(this IEnumerable<T> source, char separator)
            => string.Join(separator.ToString(), source);

        /// <summary>
        /// Concatenates the elements of the given sequence, using the specified separator between each element or member.
        /// </summary>
        /// <typeparam name="T">Type of the elements in <paramref name="source"/> sequence.</typeparam>
        /// <param name="source">A sequence of items to be joined in a string.</param>
        /// <param name="separator">A string to separate the invidual elements.</param>
        /// <returns>Joined string with separators between the elements.</returns>
        [Pure]
        public static string JoinToString<T>(this IEnumerable<T> source, string separator)
            => string.Join(separator, source);
    }
}
