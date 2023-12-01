#if GENERIC_MATH
using System.Globalization;
using System.Numerics;

namespace Funcky.Extensions;

public static partial class ParseExtensions
{
    public static Option<TNumber> ParseNumberOrNone<TNumber>(this string value, NumberStyles style, IFormatProvider? provider)
        where TNumber : INumberBase<TNumber>
        => TNumber.TryParse(value, style, provider, out var result)
            ? result
            : Option<TNumber>.None;

    public static Option<TNumber> ParseNumberOrNone<TNumber>(this ReadOnlySpan<char> value, NumberStyles style, IFormatProvider? provider)
        where TNumber : INumberBase<TNumber>
        => TNumber.TryParse(value, style, provider, out var result)
            ? result
            : Option<TNumber>.None;
}
#endif
