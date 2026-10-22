#if ORDERED_DICTIONARY
using static Funcky.Internal.ValueMapper;

namespace Funcky.Extensions;

public static class OrderedDictionaryExtensions
{
    /// <param name="dictionary">The dictionary to search.</param>
    extension<TKey, TValue>(OrderedDictionary<TKey, TValue> dictionary)
        where TKey : notnull
    {
        /// <summary>Determines the index of a specific key in the <see cref="System.Collections.Generic.OrderedDictionary{TKey,TValue}" />.</summary>
        /// <param name="key">The key to locate.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key" /> is <see langword="null" />.</exception>
        /// <returns>The index of <paramref name="key" /> if found; otherwise, <see cref="Option{TItem}.None"/>.</returns>
        public Option<int> IndexOfOrNone(TKey key)
            => MapNotFoundToNone(dictionary.IndexOf(key));
    }
}
#endif
