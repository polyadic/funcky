namespace Funcky.Extensions;

public static partial class ParseExtensions
{
#if GENERIC_PARSABLE
    /// <summary>Parses any <see cref="ISpanParsable{TSelf}"/> using the current culture; returns None if the text is not valid.</summary>
    [Pure]
    public static Option<TParsable> ParseOrNone<TParsable>(this ReadOnlySpan<char> value)
        where TParsable : ISpanParsable<TParsable>
        => value.ParseOrNone<TParsable>(provider: null);

    /// <summary>Parses any <see cref="ISpanParsable{TSelf}"/>; returns None if the text is not valid.</summary>
    /// <param name="value">The text to parse.</param>
    /// <param name="provider">Culture-specific formatting information, or <see langword="null"/> for the current culture.</param>
    [Pure]
    public static Option<TParsable> ParseOrNone<TParsable>(this ReadOnlySpan<char> value, IFormatProvider? provider)
        where TParsable : ISpanParsable<TParsable>
        => TParsable.TryParse(value, provider, out var result)
            ? result
            : Option<TParsable>.None;

    /// <summary>Parses any <see cref="IParsable{TSelf}"/> using the current culture; returns None if the text is <see langword="null"/> or not valid.</summary>
    [Pure]
    public static Option<TParsable> ParseOrNone<TParsable>(this string? value)
        where TParsable : IParsable<TParsable>
        => value.ParseOrNone<TParsable>(provider: null);

    /// <summary>Parses any <see cref="IParsable{TSelf}"/>; returns None if the text is <see langword="null"/> or not valid.</summary>
    /// <param name="value">The text to parse.</param>
    /// <param name="provider">Culture-specific formatting information, or <see langword="null"/> for the current culture.</param>
    [Pure]
    public static Option<TParsable> ParseOrNone<TParsable>(this string? value, IFormatProvider? provider)
        where TParsable : IParsable<TParsable>
        => TParsable.TryParse(value, provider, out var result)
            ? result
            : Option<TParsable>.None;
#endif

#if UTF8_SPAN_PARSABLE
    /// <summary>Parses any <see cref="IUtf8SpanParsable{TSelf}"/> from UTF-8 text using the current culture; returns None if the text is not valid.</summary>
    [Pure]
    public static Option<TParsable> ParseOrNone<TParsable>(this ReadOnlySpan<byte> utf8Text)
        where TParsable : IUtf8SpanParsable<TParsable>
        => utf8Text.ParseOrNone<TParsable>(provider: null);

    /// <summary>Parses any <see cref="IUtf8SpanParsable{TSelf}"/> from UTF-8 text; returns None if the text is not valid.</summary>
    /// <param name="utf8Text">The UTF-8 text to parse.</param>
    /// <param name="provider">Culture-specific formatting information, or <see langword="null"/> for the current culture.</param>
    [Pure]
    public static Option<TParsable> ParseOrNone<TParsable>(this ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
        where TParsable : IUtf8SpanParsable<TParsable>
        => TParsable.TryParse(utf8Text, provider, out var result)
            ? result
            : Option<TParsable>.None;
#endif
}
