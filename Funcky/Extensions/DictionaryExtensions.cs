namespace Funcky.Extensions
{
    public static partial class DictionaryExtensions
    {
        [Pure]
        public static Option<TValue> GetValueOrNone<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
            where TKey : notnull
            where TValue : notnull
            => dictionary.TryGetValue(key, out var result)
                ? result
                : Option<TValue>.None;

        [Pure]
        public static Option<TValue> GetValueOrNone<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey readOnlyKey)
            where TKey : notnull
            where TValue : notnull
            => dictionary.TryGetValue(readOnlyKey, out var result)
                ? result
                : Option<TValue>.None;
    }
}
