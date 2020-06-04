using System.Collections.Generic;
using Funcky.Monads;

namespace Funcky.Extensions
{
    public static class DictionaryExtensions
    {
        public static Option<TValue> TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
            where TKey : notnull
            where TValue : notnull
        {
            return dictionary.TryGetValue(key, out var result)
                ? Option.Some(result)
                : Option<TValue>.None();
        }

        public static Option<TValue> TryGetValue<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey readOnlyKey)
            where TKey : notnull
            where TValue : notnull
        {
            return dictionary.TryGetValue(readOnlyKey, out var result)
                ? Option.Some(result)
                : Option<TValue>.None();
        }
    }
}
