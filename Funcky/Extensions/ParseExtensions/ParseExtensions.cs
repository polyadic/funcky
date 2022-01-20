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
            => FailToOption<TEnum>.FromTryPattern(Enum.TryParse, candidate);

        [Pure]
        public static Option<TEnum> ParseEnumOrNone<TEnum>(this string candidate, bool ignoreCase)
            where TEnum : struct
            => FailToOption<TEnum>.FromTryPattern(Enum.TryParse, candidate, ignoreCase);
    }
}
