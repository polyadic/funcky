#if GENERIC_PARSABLE
namespace Funcky.Extensions;

/// <summary>The static form of <see cref="ParseExtensions.ParseOrNone{TParsable}(string?)"/>: <c>MyType.ParseOrNone(text)</c> for any type implementing the static abstract parsing interfaces.</summary>
public static class ParsableParseExtensions
{
    extension<TParsable>(TParsable)
        where TParsable : IParsable<TParsable>
    {
        /// <summary>Parses any <see cref="IParsable{TSelf}"/> using the current culture; returns None if the text is <see langword="null"/> or not valid.</summary>
        [Pure]
        public static Option<TParsable> ParseOrNone(string? candidate)
            => TParsable.TryParse(candidate, provider: null, out var result)
                ? result
                : Option<TParsable>.None;

        /// <summary>Parses any <see cref="IParsable{TSelf}"/>; returns None if the text is <see langword="null"/> or not valid.</summary>
        /// <param name="candidate">The text to parse.</param>
        /// <param name="provider">Culture-specific formatting information, or <see langword="null"/> for the current culture.</param>
        [Pure]
        public static Option<TParsable> ParseOrNone(string? candidate, IFormatProvider? provider)
            => TParsable.TryParse(candidate, provider, out var result)
                ? result
                : Option<TParsable>.None;
    }

    extension<TSpanParsable>(TSpanParsable)
        where TSpanParsable : ISpanParsable<TSpanParsable>
    {
        /// <summary>Parses any <see cref="ISpanParsable{TSelf}"/> using the current culture; returns None if the text is not valid.</summary>
        [Pure]
        public static Option<TSpanParsable> ParseOrNone(ReadOnlySpan<char> candidate)
            => TSpanParsable.TryParse(candidate, provider: null, out var result)
                ? result
                : Option<TSpanParsable>.None;

        /// <summary>Parses any <see cref="ISpanParsable{TSelf}"/>; returns None if the text is not valid.</summary>
        /// <param name="candidate">The text to parse.</param>
        /// <param name="provider">Culture-specific formatting information, or <see langword="null"/> for the current culture.</param>
        [Pure]
        public static Option<TSpanParsable> ParseOrNone(ReadOnlySpan<char> candidate, IFormatProvider? provider)
            => TSpanParsable.TryParse(candidate, provider, out var result)
                ? result
                : Option<TSpanParsable>.None;
    }

#if UTF8_SPAN_PARSABLE
    extension<TUtf8SpanParsable>(TUtf8SpanParsable)
        where TUtf8SpanParsable : IUtf8SpanParsable<TUtf8SpanParsable>
    {
        /// <summary>Parses any <see cref="IUtf8SpanParsable{TSelf}"/> from UTF-8 text using the current culture; returns None if the text is not valid.</summary>
        [Pure]
        public static Option<TUtf8SpanParsable> ParseOrNone(ReadOnlySpan<byte> utf8Text)
            => TUtf8SpanParsable.TryParse(utf8Text, provider: null, out var result)
                ? result
                : Option<TUtf8SpanParsable>.None;

        /// <summary>Parses any <see cref="IUtf8SpanParsable{TSelf}"/> from UTF-8 text; returns None if the text is not valid.</summary>
        /// <param name="utf8Text">The UTF-8 text to parse.</param>
        /// <param name="provider">Culture-specific formatting information, or <see langword="null"/> for the current culture.</param>
        [Pure]
        public static Option<TUtf8SpanParsable> ParseOrNone(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
            => TUtf8SpanParsable.TryParse(utf8Text, provider, out var result)
                ? result
                : Option<TUtf8SpanParsable>.None;
    }
#endif
}
#endif
