#if !NET6_0_OR_GREATER
using System.Collections;

namespace System.Linq;

public static class EnumerableCompatibility
{
    public static bool TryGetNonEnumeratedCount<TSource>(this IEnumerable<TSource> source, out int count)
    {
        if (source is ICollection<TSource> collectionOfT)
        {
            count = collectionOfT.Count;
            return true;
        }

        if (source is ICollection collection)
        {
            count = collection.Count;
            return true;
        }

        count = 0;
        return false;
    }
}
#endif
