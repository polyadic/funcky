using System.Text;

namespace Funcky.Async.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <summary>
    /// Concatenates the elements of the given sequence, using the specified separator between each element or member.
    /// </summary>
    /// <typeparam name="T">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    /// <param name="source">A sequence of items to be joined in a string.</param>
    /// <param name="separator">A single character to separate the individual elements.</param>
    /// <returns>Joined string with separators between the elements.</returns>
    [Pure]
    public static Task<string> JoinToStringAsync<T>(this IAsyncEnumerable<T> source, char separator)
        => JoinToStringInternal(separator.ToString(), source);

    /// <summary>
    /// Concatenates the elements of the given sequence, using the specified separator between each element or member.
    /// </summary>
    /// <typeparam name="T">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    /// <param name="source">A sequence of items to be joined in a string.</param>
    /// <param name="separator">A string to separate the individual elements.</param>
    /// <returns>Joined string with separators between the elements.</returns>
    [Pure]
    public static Task<string> JoinToStringAsync<T>(this IAsyncEnumerable<T> source, string separator)
        => JoinToStringInternal(separator, source);

    private static async Task<string> JoinToStringInternal<T>(string separator, IAsyncEnumerable<T> values)
    {
        var result = new StringBuilder();
        var enumerator = values.GetAsyncEnumerator();

        if (await enumerator.MoveNextAsync().ConfigureAwait(false))
        {
            result.Append(enumerator.Current);
        }

        while (await enumerator.MoveNextAsync().ConfigureAwait(false))
        {
            result.Append(separator);
            result.Append(enumerator.Current);
        }

        return result.ToString();
    }
}