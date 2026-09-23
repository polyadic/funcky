using System.Collections.Immutable;

namespace Funcky.Extensions;

public static partial class EnumerableExtensions
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
    public static IEnumerable<IReadOnlyList<TSource>> Transpose<TSource>(this IEnumerable<IEnumerable<TSource>> source)
    {
        var rows = new List<IEnumerator<TSource>>();

        try
        {
            foreach (var row in source)
            {
                rows.Add(row.GetEnumerator());
            }

            while (MoveToNextColumn(rows))
            {
                yield return rows.Select(row => row.Current).ToImmutableList();
            }
        }
        finally
        {
            rows.ForEach(row => row.Dispose());
        }
    }

    private static bool MoveToNextColumn<TSource>(List<IEnumerator<TSource>> rows)
    {
        var advancedRows = 0;

        foreach (var row in rows)
        {
            if (row.MoveNext())
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
