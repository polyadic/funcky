using System.Globalization;
using Funcky.Internal;

namespace Funcky.Extensions
{
    public static partial class ParseExtensions
    {
        [Pure]
        [OrNoneFromTryPattern(typeof(byte), nameof(byte.TryParse))]
        public static partial Option<byte> ParseByteOrNone(this string candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(byte), nameof(byte.TryParse))]
        public static partial Option<byte> ParseByteOrNone(this string candidate, NumberStyles styles, IFormatProvider provider);

        [Pure]
        [OrNoneFromTryPattern(typeof(sbyte), nameof(byte.TryParse))]
        public static partial Option<sbyte> ParseSByteOrNone(this string candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(sbyte), nameof(byte.TryParse))]
        public static partial Option<sbyte> ParseSByteOrNone(this string candidate, NumberStyles styles, IFormatProvider provider);

        [Pure]
        [OrNoneFromTryPattern(typeof(short), nameof(short.TryParse))]
        public static partial Option<short> ParseInt16OrNone(this string candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(short), nameof(short.TryParse))]
        public static partial Option<short> ParseInt16OrNone(this string candidate, NumberStyles styles, IFormatProvider provider);

        [Pure]
        [OrNoneFromTryPattern(typeof(ushort), nameof(ushort.TryParse))]
        public static partial Option<ushort> ParseUInt16OrNone(this string candidate, NumberStyles styles, IFormatProvider provider);

        [Pure]
        [OrNoneFromTryPattern(typeof(ushort), nameof(ushort.TryParse))]
        public static partial Option<ushort> ParseUInt16OrNone(this string candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(int), nameof(int.TryParse))]
        public static partial Option<int> ParseInt32OrNone(this string candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(uint), nameof(uint.TryParse))]
        public static partial Option<uint> ParseUInt32OrNone(this string candidate, NumberStyles styles, IFormatProvider provider);

        [Pure]
        [OrNoneFromTryPattern(typeof(uint), nameof(uint.TryParse))]
        public static partial Option<uint> ParseUInt32OrNone(this string candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(int), nameof(int.TryParse))]
        public static partial Option<int> ParseInt32OrNone(this string candidate, NumberStyles styles, IFormatProvider provider);

        [Pure]
        [OrNoneFromTryPattern(typeof(long), nameof(long.TryParse))]
        public static partial Option<long> ParseInt64OrNone(this string candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(long), nameof(long.TryParse))]
        public static partial Option<long> ParseInt64OrNone(this string candidate, NumberStyles styles, IFormatProvider provider);

        [Pure]
        [OrNoneFromTryPattern(typeof(ulong), nameof(ulong.TryParse))]
        public static partial Option<ulong> ParseUInt64OrNone(this string candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(ulong), nameof(ulong.TryParse))]
        public static partial Option<ulong> ParseUInt64OrNone(this string candidate, NumberStyles styles, IFormatProvider provider);

        [Pure]
        [OrNoneFromTryPattern(typeof(float), nameof(float.TryParse))]
        public static partial Option<float> ParseSingleOrNone(this string candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(float), nameof(float.TryParse))]
        public static partial Option<float> ParseSingleOrNone(this string candidate, NumberStyles styles, IFormatProvider provider);

        [Pure]
        [OrNoneFromTryPattern(typeof(double), nameof(double.TryParse))]
        public static partial Option<double> ParseDoubleOrNone(this string candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(double), nameof(double.TryParse))]
        public static partial Option<double> ParseDoubleOrNone(this string candidate, NumberStyles styles, IFormatProvider provider);

        [Pure]
        [OrNoneFromTryPattern(typeof(decimal), nameof(decimal.TryParse))]
        public static partial Option<decimal> ParseDecimalOrNone(this string candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(decimal), nameof(decimal.TryParse))]
        public static partial Option<decimal> ParseDecimalOrNone(this string candidate, NumberStyles styles, IFormatProvider provider);

#if READ_ONLY_SPAN_SUPPORTED
        [Pure]
        [OrNoneFromTryPattern(typeof(byte), nameof(byte.TryParse))]
        public static partial Option<byte> ParseByteOrNone(this ReadOnlySpan<char> candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(byte), nameof(byte.TryParse))]
        public static partial Option<byte> ParseByteOrNone(this ReadOnlySpan<char> candidate, NumberStyles styles, IFormatProvider provider);

        [Pure]
        [OrNoneFromTryPattern(typeof(sbyte), nameof(byte.TryParse))]
        public static partial Option<sbyte> ParseSByteOrNone(this ReadOnlySpan<char> candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(sbyte), nameof(byte.TryParse))]
        public static partial Option<sbyte> ParseSByteOrNone(this ReadOnlySpan<char> candidate, NumberStyles styles, IFormatProvider provider);

        [Pure]
        [OrNoneFromTryPattern(typeof(short), nameof(short.TryParse))]
        public static partial Option<short> ParseInt16OrNone(this ReadOnlySpan<char> candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(short), nameof(short.TryParse))]
        public static partial Option<short> ParseInt16OrNone(this ReadOnlySpan<char> candidate, NumberStyles styles, IFormatProvider provider);

        [Pure]
        [OrNoneFromTryPattern(typeof(ushort), nameof(ushort.TryParse))]
        public static partial Option<ushort> ParseUInt16OrNone(this ReadOnlySpan<char> candidate, NumberStyles styles, IFormatProvider provider);

        [Pure]
        [OrNoneFromTryPattern(typeof(ushort), nameof(ushort.TryParse))]
        public static partial Option<ushort> ParseUInt16OrNone(this ReadOnlySpan<char> candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(int), nameof(int.TryParse))]
        public static partial Option<int> ParseInt32OrNone(this ReadOnlySpan<char> candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(uint), nameof(uint.TryParse))]
        public static partial Option<uint> ParseUInt32OrNone(this ReadOnlySpan<char> candidate, NumberStyles styles, IFormatProvider provider);

        [Pure]
        [OrNoneFromTryPattern(typeof(uint), nameof(uint.TryParse))]
        public static partial Option<uint> ParseUInt32OrNone(this ReadOnlySpan<char> candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(int), nameof(int.TryParse))]
        public static partial Option<int> ParseInt32OrNone(this ReadOnlySpan<char> candidate, NumberStyles styles, IFormatProvider provider);

        [Pure]
        [OrNoneFromTryPattern(typeof(long), nameof(long.TryParse))]
        public static partial Option<long> ParseInt64OrNone(this ReadOnlySpan<char> candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(long), nameof(long.TryParse))]
        public static partial Option<long> ParseInt64OrNone(this ReadOnlySpan<char> candidate, NumberStyles styles, IFormatProvider provider);

        [Pure]
        [OrNoneFromTryPattern(typeof(ulong), nameof(ulong.TryParse))]
        public static partial Option<ulong> ParseUInt64OrNone(this ReadOnlySpan<char> candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(ulong), nameof(ulong.TryParse))]
        public static partial Option<ulong> ParseUInt64OrNone(this ReadOnlySpan<char> candidate, NumberStyles styles, IFormatProvider provider);

        [Pure]
        [OrNoneFromTryPattern(typeof(float), nameof(float.TryParse))]
        public static partial Option<float> ParseSingleOrNone(this ReadOnlySpan<char> candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(float), nameof(float.TryParse))]
        public static partial Option<float> ParseSingleOrNone(this ReadOnlySpan<char> candidate, NumberStyles styles, IFormatProvider provider);

        [Pure]
        [OrNoneFromTryPattern(typeof(double), nameof(double.TryParse))]
        public static partial Option<double> ParseDoubleOrNone(this ReadOnlySpan<char> candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(double), nameof(double.TryParse))]
        public static partial Option<double> ParseDoubleOrNone(this ReadOnlySpan<char> candidate, NumberStyles styles, IFormatProvider provider);

        [Pure]
        [OrNoneFromTryPattern(typeof(decimal), nameof(decimal.TryParse))]
        public static partial Option<decimal> ParseDecimalOrNone(this ReadOnlySpan<char> candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(decimal), nameof(decimal.TryParse))]
        public static partial Option<decimal> ParseDecimalOrNone(this ReadOnlySpan<char> candidate, NumberStyles styles, IFormatProvider provider);
#endif
    }
}
