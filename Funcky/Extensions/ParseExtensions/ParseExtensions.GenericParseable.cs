namespace Funcky.Extensions;

public static partial class ParseExtensions
{
#if GENERIC_PARSABLE
    /// <param name="value">The text to parse.</param>
    extension(ReadOnlySpan<char> value)
    {
        /// <summary>Parses any <see cref="ISpanParsable{TSelf}"/> using the current culture; returns None if the text is not valid.</summary>
        [Pure]
        public Option<TParsable> ParseOrNone<TParsable>()
            where TParsable : ISpanParsable<TParsable>
            => value.ParseOrNone<TParsable>(provider: null);

        /// <summary>Parses any <see cref="ISpanParsable{TSelf}"/>; returns None if the text is not valid.</summary>
        /// <param name="provider">Culture-specific formatting information, or <see langword="null"/> for the current culture.</param>
        [Pure]
        public Option<TParsable> ParseOrNone<TParsable>(IFormatProvider? provider)
            where TParsable : ISpanParsable<TParsable>
            => TParsable.TryParse(value, provider, out var result)
                ? result
                : Option<TParsable>.None;
    }

    /// <param name="value">The text to parse.</param>
    extension(string? value)
    {
        /// <summary>Parses any <see cref="IParsable{TSelf}"/> using the current culture; returns None if the text is <see langword="null"/> or not valid.</summary>
        [Pure]
        public Option<TParsable> ParseOrNone<TParsable>()
            where TParsable : IParsable<TParsable>
            => value.ParseOrNone<TParsable>(provider: null);

        /// <summary>Parses any <see cref="IParsable{TSelf}"/>; returns None if the text is <see langword="null"/> or not valid.</summary>
        /// <param name="provider">Culture-specific formatting information, or <see langword="null"/> for the current culture.</param>
        [Pure]
        public Option<TParsable> ParseOrNone<TParsable>(IFormatProvider? provider)
            where TParsable : IParsable<TParsable>
            => TParsable.TryParse(value, provider, out var result)
                ? result
                : Option<TParsable>.None;
    }
#endif

#if UTF8_SPAN_PARSABLE
    /// <param name="utf8Text">The UTF-8 text to parse.</param>
    extension(ReadOnlySpan<byte> utf8Text)
    {
        /// <summary>Parses any <see cref="IUtf8SpanParsable{TSelf}"/> from UTF-8 text using the current culture; returns None if the text is not valid.</summary>
        [Pure]
        public Option<TParsable> ParseOrNone<TParsable>()
            where TParsable : IUtf8SpanParsable<TParsable>
            => utf8Text.ParseOrNone<TParsable>(provider: null);

        /// <summary>Parses any <see cref="IUtf8SpanParsable{TSelf}"/> from UTF-8 text; returns None if the text is not valid.</summary>
        /// <param name="provider">Culture-specific formatting information, or <see langword="null"/> for the current culture.</param>
        [Pure]
        public Option<TParsable> ParseOrNone<TParsable>(IFormatProvider? provider)
            where TParsable : IUtf8SpanParsable<TParsable>
            => TParsable.TryParse(utf8Text, provider, out var result)
                ? result
                : Option<TParsable>.None;
    }
#endif
}
