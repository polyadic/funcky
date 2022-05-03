namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    /// <summary>Returns the items from <paramref name="source"/> when it has any or otherwise the items from <paramref name="fallback"/>.</summary>
    [Pure]
    public static IEnumerable<TSource> AnyOrElse<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> fallback)
        => AnyOrElse(source, () => fallback);

    /// <summary>Returns the items from <paramref name="source"/> when it has any or otherwise the items from <paramref name="fallback"/>.</summary>
    [Pure]
    public static IEnumerable<TSource> AnyOrElse<TSource>(this IEnumerable<TSource> source, Func<IEnumerable<TSource>> fallback)
        => source switch
        {
#if NET6_0_OR_GREATER
            _ when source.TryGetNonEnumeratedCount(out var count) => count > 0 ? source : fallback(),
#else
            ICollection<TSource> collection => collection.Count > 0 ? collection : fallback(),
#endif
            _ => AnyOrElseInternal(source, fallback),
        };

    private static IEnumerable<TSource> AnyOrElseInternal<TSource>(IEnumerable<TSource> source, Func<IEnumerable<TSource>> fallback)
    {
        var hasItems = false;

        foreach (var item in source)
        {
            hasItems = true;
            yield return item;
        }

        if (!hasItems)
        {
            foreach (var item in fallback())
            {
                yield return item;
            }
        }
    }
}
