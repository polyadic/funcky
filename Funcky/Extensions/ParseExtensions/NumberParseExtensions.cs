#if GENERIC_MATH
using System.Globalization;
using System.Numerics;

namespace Funcky.Extensions;

/// <summary>The static form of <see cref="ParseExtensions.ParseNumberOrNone{TNumber}(string?, NumberStyles, IFormatProvider?)"/>: <c>MyNumber.ParseOrNone(text, style, provider)</c>. The overloads without a <see cref="NumberStyles"/> are the ones of <see cref="ParsableParseExtensions"/>.</summary>
public static class NumberParseExtensions
{
    extension<TNumber>(TNumber)
        where TNumber : INumberBase<TNumber>
    {
        /// <summary>Parses any <see cref="INumberBase{TSelf}"/>; returns None if the text is <see langword="null"/> or not valid.</summary>
        /// <param name="candidate">The text to parse.</param>
        /// <param name="style">The permitted number format.</param>
        /// <param name="provider">Culture-specific formatting information, or <see langword="null"/> for the current culture.</param>
        [Pure]
        public static Option<TNumber> ParseOrNone(string? candidate, NumberStyles style, IFormatProvider? provider)
            => TNumber.TryParse(candidate, style, provider, out var result)
                ? result
                : Option<TNumber>.None;

        /// <summary>Parses any <see cref="INumberBase{TSelf}"/>; returns None if the text is not valid.</summary>
        /// <param name="candidate">The text to parse.</param>
        /// <param name="style">The permitted number format.</param>
        /// <param name="provider">Culture-specific formatting information, or <see langword="null"/> for the current culture.</param>
        [Pure]
        public static Option<TNumber> ParseOrNone(ReadOnlySpan<char> candidate, NumberStyles style, IFormatProvider? provider)
            => TNumber.TryParse(candidate, style, provider, out var result)
                ? result
                : Option<TNumber>.None;
    }
}
#endif
