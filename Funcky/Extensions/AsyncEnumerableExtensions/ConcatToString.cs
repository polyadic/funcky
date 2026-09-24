#if INTEGRATED_ASYNC
using System.Text;

namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <summary>
    /// Concatenates the elements of the given sequence to a single string.
    /// </summary>
    /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    /// <returns>Concatenated string.</returns>
    [Pure]
    public static async ValueTask<string> ConcatToStringAsync<TSource>(this IAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default)
    {
        var result = new StringBuilder();

        await source.AggregateAsync(result, (builder, value) => builder.Append(value), cancellationToken).ConfigureAwait(false);

        return result.ToString();
    }
}
#endif
