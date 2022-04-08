using System.Globalization;
using System.Numerics;
using Funcky.Internal;

namespace Funcky.Extensions
{
    public static partial class ParseExtensions
    {
        [Pure]
        [OrNoneFromTryPattern(typeof(BigInteger), nameof(BigInteger.TryParse))]
        public static partial Option<BigInteger> ParseBigIntegerOrNone(this string? candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(BigInteger), nameof(BigInteger.TryParse))]
        public static partial Option<BigInteger> ParseBigIntegerOrNone(this string? candidate, NumberStyles style, IFormatProvider? provider);

#if READ_ONLY_SPAN_SUPPORTED
        [Pure]
        [OrNoneFromTryPattern(typeof(BigInteger), nameof(BigInteger.TryParse))]
        public static partial Option<BigInteger> ParseBigIntegerOrNone(this ReadOnlySpan<char> candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(BigInteger), nameof(BigInteger.TryParse))]
        public static partial Option<BigInteger> ParseBigIntegerOrNone(this ReadOnlySpan<char> candidate, NumberStyles style, IFormatProvider? provider);
#endif
    }
}
