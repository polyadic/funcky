namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    /// <summary>Returns the minimum value in a generic sequence according to a specified key selector function.</summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
    /// <typeparam name="TKey">The type of key to compare elements by.</typeparam>
    /// <param name="source">A sequence of values to determine the minimum value of.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <returns>The value with the minimum key in the sequence or None.</returns>
    public static Option<TSource> MinByOrNone<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        where TSource : notnull
        => MinByOrNone(source, keySelector, Comparer<TKey>.Default);

    /// <summary>Returns the minimum value in a generic sequence according to a specified key selector function.</summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
    /// <typeparam name="TKey">The type of key to compare elements by.</typeparam>
    /// <param name="source">A sequence of values to determine the minimum value of.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <param name="comparer">The <see cref="IComparer{TKey}" /> to compare keys.</param>
    /// <returns>The value with the minimum key in the sequence or None.</returns>
    [Pure]
    public static Option<TSource> MinByOrNone<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        where TSource : notnull
    {
        using var enumerator = source.GetEnumerator();

        if (!enumerator.MoveNext())
        {
            return Option<TSource>.None;
        }

        var minimum = enumerator.Current;
        while (enumerator.MoveNext())
        {
            var nextValue = enumerator.Current;
            if (comparer.Compare(keySelector(nextValue), keySelector(minimum)) < 0)
            {
                minimum = nextValue;
            }
        }

        return minimum;
    }
}
