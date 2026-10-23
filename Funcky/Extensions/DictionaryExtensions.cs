using System.Runtime.CompilerServices;

namespace Funcky.Extensions;

public static class DictionaryExtensions
{
    extension<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
        where TKey : notnull
        where TValue : notnull
    {
        [Pure]
        public Option<TValue> GetValueOrNone(TKey key)
            => dictionary.TryGetValue(key, out var result)
                ? result
                : Option<TValue>.None;
    }

    extension<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> dictionary)
        where TKey : notnull
        where TValue : notnull
    {
        [Pure]
        [OverloadResolutionPriority(1)]
        public Option<TValue> GetValueOrNone(TKey readOnlyKey)
            => dictionary.TryGetValue(readOnlyKey, out var result)
                ? result
                : Option<TValue>.None;
    }

#if REMOVE_EXTENSION
    extension<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
        where TKey : notnull
        where TValue : notnull
    {
        public Option<TValue> RemoveOrNone(TKey key)
            => dictionary.Remove(key, out var result)
                ? result
                : Option<TValue>.None;
    }
#endif
}
