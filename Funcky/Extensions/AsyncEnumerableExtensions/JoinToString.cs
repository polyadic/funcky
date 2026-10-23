#if ASYNC_SUPPORTED
using System.Text;

namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <param name="source">A sequence of items to be joined in a string.</param>
    /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    extension<TSource>(IAsyncEnumerable<TSource> source)
    {
        /// <summary>
        /// Concatenates the elements of the given sequence, using the specified separator between each element or member.
        /// </summary>
        /// <param name="separator">A single character to separate the individual elements.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>Joined string with separators between the elements.</returns>
        [Pure]
        public ValueTask<string> JoinToStringAsync(char separator, CancellationToken cancellationToken = default)
            => JoinToStringInternal(separator.ToString(), source, cancellationToken);

        /// <summary>
        /// Concatenates the elements of the given sequence, using the specified separator between each element or member.
        /// </summary>
        /// <param name="separator">A string to separate the individual elements.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>Joined string with separators between the elements.</returns>
        [Pure]
        public ValueTask<string> JoinToStringAsync(string separator, CancellationToken cancellationToken = default)
            => JoinToStringInternal(separator, source, cancellationToken);
    }

    private static async ValueTask<string> JoinToStringInternal<T>(string separator, IAsyncEnumerable<T> values, CancellationToken cancellationToken = default)
    {
        var result = new StringBuilder();
        var enumerator = values.GetAsyncEnumerator(cancellationToken);

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
#endif
