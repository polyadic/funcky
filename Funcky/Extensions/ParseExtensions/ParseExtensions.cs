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
    }
}
