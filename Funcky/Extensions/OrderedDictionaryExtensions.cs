#if ORDERED_DICTIONARY
using static Funcky.Internal.ValueMapper;

namespace Funcky.Extensions;

public static class OrderedDictionaryExtensions
{
    /// <summary>Determines the index of a specific key in the <see cref="System.Collections.Generic.OrderedDictionary{TKey,TValue}" />.</summary>
    /// <param name="dictionary">The dictionary to search.</param>
    /// <param name="key">The key to locate.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="key" /> is <see langword="null" />.</exception>
    /// <returns>The index of <paramref name="key" /> if found; otherwise, <see cref="Option{TItem}.None"/>.</returns>
    public static Option<int> IndexOfOrNone<TKey, TValue>(this OrderedDictionary<TKey, TValue> dictionary, TKey key)
        where TKey : notnull
        => MapNotFoundToNone(dictionary.IndexOf(key));
}
#endif
