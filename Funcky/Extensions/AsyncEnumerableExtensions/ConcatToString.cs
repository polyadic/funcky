#if ASYNC_SUPPORTED
using System.Text;

namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    extension<TSource>(IAsyncEnumerable<TSource> source)
    {
        /// <summary>
        /// Concatenates the elements of the given sequence to a single string.
        /// </summary>
        /// <returns>Concatenated string.</returns>
        [Pure]
        public async ValueTask<string> ConcatToStringAsync(CancellationToken cancellationToken = default)
        {
            var result = new StringBuilder();

            await source.AggregateAsync(result, (builder, value) => builder.Append(value), cancellationToken).ConfigureAwait(false);

            return result.ToString();
        }
    }
}
#endif
