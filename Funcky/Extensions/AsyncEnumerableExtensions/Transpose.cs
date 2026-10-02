#if INTEGRATED_ASYNC
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <summary>
    /// On a rectangular matrix (sequence of sequences where every inner sequence is of the same length) this extension function produces the transposed matrix (rows and columns switched).
    /// </summary>
    /// <remarks>
    /// The columns are yielded lazily: the outer sequence is enumerated once when the first column is requested,
    /// and every row is advanced by exactly one element per column.
    /// </remarks>
    /// <param name="source">A source matrix.</param>
    /// <typeparam name="TSource">The type of the elements of the source matrix.</typeparam>
    /// <returns>A lazy transposition of the matrix.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the rows of <paramref name="source"/> do not all have the same length.</exception>
    [Pure]
    public static IAsyncEnumerable<IEnumerable<TSource>> Transpose<TSource>(this IEnumerable<IAsyncEnumerable<TSource>> source)
        => TransposeInternal(source);

    private static async IAsyncEnumerable<IEnumerable<TSource>> TransposeInternal<TSource>(
        IEnumerable<IAsyncEnumerable<TSource>> source,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var rows = new List<IAsyncEnumerator<TSource>>();

        try
        {
            foreach (var row in source)
            {
                rows.Add(row.GetAsyncEnumerator(cancellationToken));
            }

            while (await MoveToNextColumnAsync(rows).ConfigureAwait(false))
            {
                yield return rows.Select(row => row.Current).ToImmutableList();
            }
        }
        finally
        {
            foreach (var row in rows)
            {
                await DisposeEnumerator(row).ConfigureAwait(false);
            }
        }
    }

    private static async ValueTask<bool> MoveToNextColumnAsync<TSource>(List<IAsyncEnumerator<TSource>> rows)
    {
        var advancedRows = 0;

        foreach (var row in rows)
        {
            if (await row.MoveNextAsync().ConfigureAwait(false))
            {
                advancedRows++;
            }
        }

        return advancedRows == 0
            ? false
            : advancedRows == rows.Count
                ? true
                : throw new InvalidOperationException("Transpose requires a rectangular matrix, but the rows do not all have the same length.");
    }
}
#endif
