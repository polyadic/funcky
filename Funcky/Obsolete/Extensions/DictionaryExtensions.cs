using System.ComponentModel;
using System.Diagnostics.Contracts;
using Funcky.Monads;

namespace Funcky.Extensions
{
    public static partial class DictionaryExtensions
    {
        [Obsolete("Use " + nameof(GetValueOrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        public static Option<TValue> TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
            where TKey : notnull
            where TValue : notnull
            => dictionary.GetValueOrNone(key);

        [Obsolete("Use " + nameof(GetValueOrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        public static Option<TValue> TryGetValue<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey readOnlyKey)
            where TKey : notnull
            where TValue : notnull
            => dictionary.GetValueOrNone(readOnlyKey);
    }
}
