using System.Collections.Generic;
using Funcky.Monads;

namespace Funcky.Extensions
{
    public static class DictionaryExtensions
    {
        public static Option<TValue> TryGetValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            return dictionary.TryGetValue(key, out var result)
                ? new Option<TValue>(result)
                : new Option<TValue>();
        }
    }
}