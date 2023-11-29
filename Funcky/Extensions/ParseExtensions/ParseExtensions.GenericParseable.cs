namespace Funcky.Extensions;

public static partial class ParseExtensions
{
#if GENERIC_PARSABLE
    public static Option<TParsable> ParseOrNone<TParsable>(this ReadOnlySpan<char> value, IFormatProvider? provider)
        where TParsable : ISpanParsable<TParsable>
        => TParsable.TryParse(value, provider, out var result)
            ? result
            : Option<TParsable>.None;

    public static Option<TParsable> ParseOrNone<TParsable>(this string? value, IFormatProvider? provider)
        where TParsable : IParsable<TParsable>
        => TParsable.TryParse(value, provider, out var result)
            ? result
            : Option<TParsable>.None;
#endif

#if UTF8_SPAN_PARSABLE
    public static Option<TParsable> ParseOrNone<TParsable>(this ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
        where TParsable : IUtf8SpanParsable<TParsable>
        => TParsable.TryParse(utf8Text, provider, out var result)
            ? result
            : Option<TParsable>.None;
#endif
}
