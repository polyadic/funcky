namespace Funcky.Internal;

internal static class ExtremumByOrNone
{
    private enum Extremum
    {
        Minimum,
        Maximum,
    }

    public static Option<TSource> MinByOrNone<TSource, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        where TSource : notnull
        => FindOrNone(source, keySelector, comparer, Extremum.Minimum);

    public static Option<TSource> MaxByOrNone<TSource, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        where TSource : notnull
        => FindOrNone(source, keySelector, comparer, Extremum.Maximum);

    /// <summary>Mirrors the semantics of <c>Enumerable.MinBy</c>/<c>Enumerable.MaxBy</c>: each key is computed once,
    /// and when the key type is nullable, elements with a <see langword="null"/> key are never candidates.
    /// If all keys are <see langword="null"/>, the first element is returned.</summary>
    private static Option<TSource> FindOrNone<TSource, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer, Extremum extremum)
        where TSource : notnull
    {
        using var enumerator = source.GetEnumerator();

        if (!enumerator.MoveNext())
        {
            return Option<TSource>.None;
        }

        var value = enumerator.Current;
        var key = keySelector(value);

        if (default(TKey) is null && key is null)
        {
            var firstValue = value;

            do
            {
                if (!enumerator.MoveNext())
                {
                    return firstValue;
                }

                value = enumerator.Current;
                key = keySelector(value);
            }
            while (key is null);
        }

        while (enumerator.MoveNext())
        {
            var nextValue = enumerator.Current;
            var nextKey = keySelector(nextValue);

            if (nextKey is not null && IsBetter(comparer.Compare(nextKey, key), extremum))
            {
                value = nextValue;
                key = nextKey;
            }
        }

        return value;
    }

    private static bool IsBetter(int comparison, Extremum extremum)
        => extremum switch
        {
            Extremum.Minimum => comparison < 0,
            Extremum.Maximum => comparison > 0,
            _ => throw new InvalidOperationException($"Unreachable: Enum variant {extremum} is not handled"),
        };
}
