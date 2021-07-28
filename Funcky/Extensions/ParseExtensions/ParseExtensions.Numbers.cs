using System.Globalization;
using Funcky.Internal;

namespace Funcky.Extensions
{
    public static partial class ParseExtensions
    {
        public static Option<byte> ParseByteOrNone(this string candidate)
            => FailToOption<byte>.FromTryPattern(byte.TryParse, candidate);

        [Pure]
        public static Option<byte> ParseByteOrNone(this string candidate, NumberStyles styles, IFormatProvider provider)
            => FailToOption<byte>.FromTryPattern(byte.TryParse, candidate, styles, provider);

        [Pure]
        public static Option<short> ParseShortOrNone(this string candidate)
            => FailToOption<short>.FromTryPattern(short.TryParse, candidate);

        [Pure]
        public static Option<short> ParseShortOrNone(this string candidate, NumberStyles styles, IFormatProvider provider)
            => FailToOption<short>.FromTryPattern(short.TryParse, candidate, styles, provider);

        [Pure]
        public static Option<int> ParseIntOrNone(this string candidate)
            => FailToOption<int>.FromTryPattern(int.TryParse, candidate);

        [Pure]
        public static Option<int> ParseIntOrNone(this string candidate, NumberStyles styles, IFormatProvider provider)
            => FailToOption<int>.FromTryPattern(int.TryParse, candidate, styles, provider);

        [Pure]
        public static Option<long> ParseLongOrNone(this string candidate)
            => FailToOption<long>.FromTryPattern(long.TryParse, candidate);

        [Pure]
        public static Option<long> ParseLongOrNone(this string candidate, NumberStyles styles, IFormatProvider provider)
            => FailToOption<long>.FromTryPattern(long.TryParse, candidate, styles, provider);

        [Pure]
        public static Option<float> ParseFloatOrNone(this string candidate)
            => FailToOption<float>.FromTryPattern(float.TryParse, candidate);

        [Pure]
        public static Option<float> ParseFloatOrNone(this string candidate, NumberStyles styles, IFormatProvider provider)
            => FailToOption<float>.FromTryPattern(float.TryParse, candidate, styles, provider);

        [Pure]
        public static Option<double> ParseDoubleOrNone(this string candidate)
            => FailToOption<double>.FromTryPattern(double.TryParse, candidate);

        [Pure]
        public static Option<double> ParseDoubleOrNone(this string candidate, NumberStyles styles, IFormatProvider provider)
            => FailToOption<double>.FromTryPattern(double.TryParse, candidate, styles, provider);

        [Pure]
        public static Option<decimal> ParseDecimalOrNone(this string candidate)
            => FailToOption<decimal>.FromTryPattern(decimal.TryParse, candidate);

        [Pure]
        public static Option<decimal> ParseDecimalOrNone(this string candidate, NumberStyles styles, IFormatProvider provider)
            => FailToOption<decimal>.FromTryPattern(decimal.TryParse, candidate, styles, provider);
    }
}
