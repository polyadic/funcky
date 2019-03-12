using System.Collections.Generic;
using Funcky.Monads;

namespace Funcky.Extensions
{
    public static class DictionaryExtensions
    {
        public static Maybe<TValue> TryGetValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            return dictionary.TryGetValue(key, out var result)
                ? new Maybe<TValue>(result)
                : new Maybe<TValue>();
        }
    }
}