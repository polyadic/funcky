#if GENERIC_MATH
using System.Globalization;
using System.Numerics;

namespace Funcky.Extensions;

public static partial class ParseExtensions
{
    /// <summary>Parses any <see cref="INumberBase{TSelf}"/> with the type's default <see cref="NumberStyles"/> and the current culture; returns None if the text is <see langword="null"/> or not valid.</summary>
    [Pure]
    public static Option<TNumber> ParseNumberOrNone<TNumber>(this string? value)
        where TNumber : INumberBase<TNumber>
        => value.ParseNumberOrNone<TNumber>(provider: null);

    /// <summary>Parses any <see cref="INumberBase{TSelf}"/> with the type's default <see cref="NumberStyles"/>; returns None if the text is <see langword="null"/> or not valid.</summary>
    /// <param name="value">The text to parse.</param>
    /// <param name="provider">Culture-specific formatting information, or <see langword="null"/> for the current culture.</param>
    [Pure]
    public static Option<TNumber> ParseNumberOrNone<TNumber>(this string? value, IFormatProvider? provider)
        where TNumber : INumberBase<TNumber>
        => TNumber.TryParse(value, provider, out var result)
            ? result
            : Option<TNumber>.None;

    /// <summary>Parses any <see cref="INumberBase{TSelf}"/>; returns None if the text is <see langword="null"/> or not valid.</summary>
    /// <param name="value">The text to parse.</param>
    /// <param name="style">The permitted number format.</param>
    /// <param name="provider">Culture-specific formatting information, or <see langword="null"/> for the current culture.</param>
    [Pure]
    public static Option<TNumber> ParseNumberOrNone<TNumber>(this string? value, NumberStyles style, IFormatProvider? provider)
        where TNumber : INumberBase<TNumber>
        => TNumber.TryParse(value, style, provider, out var result)
            ? result
            : Option<TNumber>.None;

    /// <summary>Parses any <see cref="INumberBase{TSelf}"/> with the type's default <see cref="NumberStyles"/> and the current culture; returns None if the text is not valid.</summary>
    [Pure]
    public static Option<TNumber> ParseNumberOrNone<TNumber>(this ReadOnlySpan<char> value)
        where TNumber : INumberBase<TNumber>
        => value.ParseNumberOrNone<TNumber>(provider: null);

    /// <summary>Parses any <see cref="INumberBase{TSelf}"/> with the type's default <see cref="NumberStyles"/>; returns None if the text is not valid.</summary>
    /// <param name="value">The text to parse.</param>
    /// <param name="provider">Culture-specific formatting information, or <see langword="null"/> for the current culture.</param>
    [Pure]
    public static Option<TNumber> ParseNumberOrNone<TNumber>(this ReadOnlySpan<char> value, IFormatProvider? provider)
        where TNumber : INumberBase<TNumber>
        => TNumber.TryParse(value, provider, out var result)
            ? result
            : Option<TNumber>.None;

    /// <summary>Parses any <see cref="INumberBase{TSelf}"/>; returns None if the text is not valid.</summary>
    /// <param name="value">The text to parse.</param>
    /// <param name="style">The permitted number format.</param>
    /// <param name="provider">Culture-specific formatting information, or <see langword="null"/> for the current culture.</param>
    [Pure]
    public static Option<TNumber> ParseNumberOrNone<TNumber>(this ReadOnlySpan<char> value, NumberStyles style, IFormatProvider? provider)
        where TNumber : INumberBase<TNumber>
        => TNumber.TryParse(value, style, provider, out var result)
            ? result
            : Option<TNumber>.None;
}
#endif
