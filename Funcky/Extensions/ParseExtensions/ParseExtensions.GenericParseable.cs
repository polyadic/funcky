namespace Funcky.Extensions;

public static partial class ParseExtensions
{
#if GENERIC_PARSEABLE
    public static Option<TParseable> ParseOrNone<TParseable>(this ReadOnlySpan<char> value, IFormatProvider? provider)
        where TParseable : ISpanParsable<TParseable>
        => TParseable.TryParse(value, provider, out var result)
            ? result
            : Option<TParseable>.None;

    public static Option<TParseable> ParseOrNone<TParseable>(this string? value, IFormatProvider? provider)
        where TParseable : IParsable<TParseable>
        => TParseable.TryParse(value, provider, out var result)
            ? result
            : Option<TParseable>.None;
#endif

#if UTF8_SPAN_PARSEABLE
    public static Option<TParseable> ParseOrNone<TParseable>(this ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
        where TParseable : IUtf8SpanParsable<TParseable>
        => TParseable.TryParse(utf8Text, provider, out var result)
            ? result
            : Option<TParseable>.None;
#endif
}
