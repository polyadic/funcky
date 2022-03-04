using Funcky.Internal;

namespace Funcky.Extensions
{
    public static partial class ParseExtensions
    {
        [Pure]
        [OrNoneFromTryPattern(typeof(bool), nameof(bool.TryParse))]
        public static partial Option<bool> ParseBooleanOrNone(this string candidate);

        [Pure]
        public static Option<TEnum> ParseEnumOrNone<TEnum>(this string candidate)
            where TEnum : struct
            => Enum.TryParse(candidate, out TEnum result)
                ? result
                : Option<TEnum>.None();

        [Pure]
        public static Option<TEnum> ParseEnumOrNone<TEnum>(this string candidate, bool ignoreCase)
            where TEnum : struct
            => Enum.TryParse(candidate, ignoreCase, out TEnum result)
                ? result
                : Option<TEnum>.None();

#if !NETSTANDARD2_0
        [Pure]
        public static Option<object> ParseEnumOrNone(this string candidate, Type type)
            => Enum.TryParse(type, candidate, out var result)
                ? result!
                : Option<object>.None();

        [Pure]
        public static Option<object> ParseEnumOrNone(this string candidate, Type type, bool ignoreCase)
            => Enum.TryParse(type, candidate, ignoreCase, out var result)
                ? result!
                : Option<object>.None();
#endif

#if READ_ONLY_SPAN_SUPPORTED
        [Pure]
        [OrNoneFromTryPattern(typeof(bool), nameof(bool.TryParse))]
        public static partial Option<bool> ParseBooleanOrNone(this ReadOnlySpan<char> candidate);

#if NET6_0_OR_GREATER
        [Pure]
        public static Option<TEnum> ParseEnumOrNone<TEnum>(this ReadOnlySpan<char> candidate)
            where TEnum : struct
            => Enum.TryParse(candidate, out TEnum result)
                ? result
                : Option<TEnum>.None();

        [Pure]
        public static Option<TEnum> ParseEnumOrNone<TEnum>(this ReadOnlySpan<char> candidate, bool ignoreCase)
            where TEnum : struct
            => Enum.TryParse(candidate, ignoreCase, out TEnum result)
                ? result
                : Option<TEnum>.None();

        [Pure]
        public static Option<object> ParseEnumOrNone(this ReadOnlySpan<char> candidate, Type type)
            => Enum.TryParse(type, candidate, out var result)
                ? result!
                : Option<object>.None();

        [Pure]
        public static Option<object> ParseEnumOrNone(this ReadOnlySpan<char> candidate, Type type, bool ignoreCase)
            => Enum.TryParse(type, candidate, ignoreCase, out var result)
                ? result!
                : Option<object>.None();
#endif
#endif
    }
}
