#if GENERIC_PARSEABLE
namespace Funcky.Extensions;

public static partial class ParseExtensions
{
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
}
#endif
