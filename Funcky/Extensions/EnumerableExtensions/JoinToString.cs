namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    /// <summary>
    /// Concatenates the elements of the given sequence, using the specified separator between each element or member.
    /// </summary>
    /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    /// <param name="source">A sequence of items to be joined in a string.</param>
    /// <param name="separator">A single character to separate the individual elements.</param>
    /// <returns>Joined string with separators between the elements.</returns>
    [Pure]
    public static string JoinToString<TSource>(this IEnumerable<TSource> source, char separator)
#if JOIN_TO_STRING_CHAR_SEPARATOR
        => string.Join(separator, source);
#else
        => string.Join(separator.ToString(), source);
#endif

    /// <summary>
    /// Concatenates the elements of the given sequence, using the specified separator between each element or member.
    /// </summary>
    /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    /// <param name="source">A sequence of items to be joined in a string.</param>
    /// <param name="separator">A string to separate the individual elements.</param>
    /// <returns>Joined string with separators between the elements.</returns>
    [Pure]
    public static string JoinToString<TSource>(this IEnumerable<TSource> source, string separator)
        => string.Join(separator, source);
}
