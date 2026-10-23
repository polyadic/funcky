namespace Funcky.Extensions;

/// <summary>The static form of <see cref="ParseExtensions.ParseEnumOrNone{TEnum}(string?)"/>: <c>MyEnum.ParseOrNone(text)</c>.</summary>
public static class EnumParseExtensions
{
    extension<TEnum>(TEnum)
        where TEnum : struct, Enum
    {
        /// <summary>Parses the name or numeric value of an enum member; returns <see cref="Option{TItem}.None"/> for <see langword="null"/> or an unknown name.</summary>
        [Pure]
        public static Option<TEnum> ParseOrNone(string? candidate)
            => Enum.TryParse(candidate, out TEnum result)
                ? result
                : Option<TEnum>.None;

        /// <summary>Parses the name or numeric value of an enum member; returns <see cref="Option{TItem}.None"/> for <see langword="null"/> or an unknown name.</summary>
        [Pure]
        public static Option<TEnum> ParseOrNone(string? candidate, bool ignoreCase)
            => Enum.TryParse(candidate, ignoreCase, out TEnum result)
                ? result
                : Option<TEnum>.None;

#if READ_ONLY_SPAN_SUPPORTED
#if NET6_0_OR_GREATER
        /// <summary>Parses the name or numeric value of an enum member; returns <see cref="Option{TItem}.None"/> for an unknown name.</summary>
        [Pure]
        public static Option<TEnum> ParseOrNone(ReadOnlySpan<char> candidate)
            => Enum.TryParse(candidate, out TEnum result)
                ? result
                : Option<TEnum>.None;

        /// <summary>Parses the name or numeric value of an enum member; returns <see cref="Option{TItem}.None"/> for an unknown name.</summary>
        [Pure]
        public static Option<TEnum> ParseOrNone(ReadOnlySpan<char> candidate, bool ignoreCase)
            => Enum.TryParse(candidate, ignoreCase, out TEnum result)
                ? result
                : Option<TEnum>.None;
#endif
#endif
    }

#if !NETSTANDARD2_0
    extension(Enum)
    {
        /// <summary>Parses the name or numeric value of a member of <c>enumType</c>; returns <see cref="Option{TItem}.None"/> for <see langword="null"/> or an unknown name.</summary>
        [Pure]
        public static Option<object> ParseOrNone(Type enumType, string? candidate)
            => Enum.TryParse(enumType, candidate, out var result)
                ? result!
                : Option<object>.None;

        /// <summary>Parses the name or numeric value of a member of <c>enumType</c>; returns <see cref="Option{TItem}.None"/> for <see langword="null"/> or an unknown name.</summary>
        [Pure]
        public static Option<object> ParseOrNone(Type enumType, string? candidate, bool ignoreCase)
            => Enum.TryParse(enumType, candidate, ignoreCase, out var result)
                ? result!
                : Option<object>.None;

#if READ_ONLY_SPAN_SUPPORTED
#if NET6_0_OR_GREATER
        /// <summary>Parses the name or numeric value of a member of <c>enumType</c>; returns <see cref="Option{TItem}.None"/> for an unknown name.</summary>
        [Pure]
        public static Option<object> ParseOrNone(Type enumType, ReadOnlySpan<char> candidate)
            => Enum.TryParse(enumType, candidate, out var result)
                ? result!
                : Option<object>.None;

        /// <summary>Parses the name or numeric value of a member of <c>enumType</c>; returns <see cref="Option{TItem}.None"/> for an unknown name.</summary>
        [Pure]
        public static Option<object> ParseOrNone(Type enumType, ReadOnlySpan<char> candidate, bool ignoreCase)
            => Enum.TryParse(enumType, candidate, ignoreCase, out var result)
                ? result!
                : Option<object>.None;
#endif
#endif
    }
#endif
}
