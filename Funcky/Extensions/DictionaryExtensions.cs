using System.Runtime.CompilerServices;

namespace Funcky.Extensions;

public static class DictionaryExtensions
{
    [Pure]
    public static Option<TValue> GetValueOrNone<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
#if NETCOREAPP3_1
        // TKey was constraint to notnull when nullability annotations were originally added. It was later dropped again.
        // See: https://github.com/dotnet/runtime/issues/31401
        where TKey : notnull
#endif
        where TValue : notnull
        => dictionary.TryGetValue(key, out var result)
            ? result
            : Option<TValue>.None;

    [Pure]
    [OverloadResolutionPriority(10)]
    public static Option<TValue> GetValueOrNone<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey readOnlyKey)
#if NETCOREAPP3_1
        // TKey was constraint to notnull when nullability annotations were originally added. It was later dropped again.
        // See: https://github.com/dotnet/runtime/issues/31401
        where TKey : notnull
#endif
        where TValue : notnull
        => dictionary.TryGetValue(readOnlyKey, out var result)
            ? result
            : Option<TValue>.None;
}
