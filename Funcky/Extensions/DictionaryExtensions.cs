using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Funcky.Monads;

namespace Funcky.Extensions
{
    public static class DictionaryExtensions
    {
        [Pure]
        public static Option<TValue> TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
            where TKey : notnull
            where TValue : notnull
            => dictionary.TryGetValue(key, out var result)
                ? Option.Some(result)
                : Option<TValue>.None();

        [Pure]
        public static Option<TValue> TryGetValue<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey readOnlyKey)
            where TKey : notnull
            where TValue : notnull
            => dictionary.TryGetValue(readOnlyKey, out var result)
                ? Option.Some(result)
                : Option<TValue>.None();
    }
}
