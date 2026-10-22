namespace Funcky.Extensions;

public static partial class ParseExtensions
{
    extension(string? candidate)
    {
        /// <summary>Parses the name or numeric value of an enum member; returns <see cref="Option{TItem}.None"/> for <see langword="null"/> or an unknown name.</summary>
        [Pure]
        public Option<TEnum> ParseEnumOrNone<TEnum>()
            where TEnum : struct, Enum
            => Enum.TryParse(candidate, out TEnum result)
                ? result
                : Option<TEnum>.None;

        [Pure]
        public Option<TEnum> ParseEnumOrNone<TEnum>(bool ignoreCase)
            where TEnum : struct, Enum
            => Enum.TryParse(candidate, ignoreCase, out TEnum result)
                ? result
                : Option<TEnum>.None;
    }

#if !NETSTANDARD2_0
    extension(string? candidate)
    {
        [Pure]
        public Option<object> ParseEnumOrNone(Type enumType)
            => Enum.TryParse(enumType, candidate, out var result)
                ? result!
                : Option<object>.None;

        [Pure]
        public Option<object> ParseEnumOrNone(Type enumType, bool ignoreCase)
            => Enum.TryParse(enumType, candidate, ignoreCase, out var result)
                ? result!
                : Option<object>.None;
    }
#endif

#if READ_ONLY_SPAN_SUPPORTED
#if NET6_0_OR_GREATER
    extension(ReadOnlySpan<char> candidate)
    {
        [Pure]
        public Option<TEnum> ParseEnumOrNone<TEnum>()
            where TEnum : struct, Enum
            => Enum.TryParse(candidate, out TEnum result)
                ? result
                : Option<TEnum>.None;

        [Pure]
        public Option<TEnum> ParseEnumOrNone<TEnum>(bool ignoreCase)
            where TEnum : struct, Enum
            => Enum.TryParse(candidate, ignoreCase, out TEnum result)
                ? result
                : Option<TEnum>.None;

        [Pure]
        public Option<object> ParseEnumOrNone(Type enumType)
            => Enum.TryParse(enumType, candidate, out var result)
                ? result!
                : Option<object>.None;

        [Pure]
        public Option<object> ParseEnumOrNone(Type enumType, bool ignoreCase)
            => Enum.TryParse(enumType, candidate, ignoreCase, out var result)
                ? result!
                : Option<object>.None;
    }
#endif
#endif
}
