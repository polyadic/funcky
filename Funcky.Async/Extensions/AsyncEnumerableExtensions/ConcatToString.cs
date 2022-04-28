using System.Text;

namespace Funcky.Async.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <summary>
    /// Concatenates the elements of the given sequence to a single string.
    /// </summary>
    /// <typeparam name="T">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    /// <returns>Concatenated string.</returns>
    [Pure]
    public static async Task<string> ConcatToString<T>(this IAsyncEnumerable<T> source)
    {
        var result = new StringBuilder();

        await source.AggregateAsync(result, (builder, value) => builder.Append(value)).ConfigureAwait(false);

        return result.ToString();
    }
}
