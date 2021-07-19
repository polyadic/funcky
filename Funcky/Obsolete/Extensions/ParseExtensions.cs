using System.ComponentModel;
using System.Globalization;

namespace Funcky.Extensions
{
    public static partial class ParseExtensions
    {
        [Obsolete("Use " + nameof(ParseBooleanOrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        public static Option<bool> TryParseBoolean(this string candidate)
            => candidate.ParseBooleanOrNone();

        [Obsolete("Use " + nameof(ParseIntOrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        public static Option<int> TryParseInt(this string candidate)
            => candidate.ParseIntOrNone();

        [Obsolete("Use " + nameof(ParseIntOrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        public static Option<int> TryParseInt(this string candidate, NumberStyles styles, IFormatProvider provider)
            => candidate.ParseIntOrNone();

        [Obsolete("Use " + nameof(ParseByteOrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        public static Option<byte> TryParseByte(this string candidate)
            => candidate.ParseByteOrNone();

        [Obsolete("Use " + nameof(ParseByteOrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        public static Option<byte> TryParseByte(this string candidate, NumberStyles styles, IFormatProvider provider)
            => candidate.ParseByteOrNone();

        [Obsolete("Use " + nameof(ParseShortOrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        public static Option<short> TryParseShort(this string candidate)
            => candidate.ParseShortOrNone();

        [Obsolete("Use " + nameof(ParseShortOrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        public static Option<short> TryParseShort(this string candidate, NumberStyles styles, IFormatProvider provider)
            => candidate.ParseShortOrNone();

        [Obsolete("Use " + nameof(ParseLongOrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        public static Option<long> TryParseLong(this string candidate)
            => candidate.ParseLongOrNone();

        [Obsolete("Use " + nameof(ParseLongOrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        public static Option<long> TryParseLong(this string candidate, NumberStyles styles, IFormatProvider provider)
            => candidate.ParseLongOrNone();

        [Obsolete("Use " + nameof(ParseDoubleOrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        public static Option<double> TryParseDouble(this string candidate)
            => candidate.ParseDoubleOrNone();

        [Obsolete("Use " + nameof(ParseDoubleOrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        public static Option<double> TryParseDouble(this string candidate, NumberStyles styles, IFormatProvider provider)
            => candidate.ParseDoubleOrNone();

        [Obsolete("Use " + nameof(ParseDecimalOrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        public static Option<decimal> TryParseDecimal(this string candidate)
            => candidate.ParseDecimalOrNone();

        [Obsolete("Use " + nameof(ParseDecimalOrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        public static Option<decimal> TryParseDecimal(this string candidate, NumberStyles styles, IFormatProvider provider)
            => candidate.ParseDecimalOrNone();

        [Obsolete("Use " + nameof(ParseDateTimeOrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        public static Option<DateTime> TryParseDateTime(this string candidate)
            => candidate.ParseDateTimeOrNone();

        [Obsolete("Use " + nameof(ParseDateTimeOrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        public static Option<DateTime> TryParseDateTime(this string candidate, IFormatProvider provider, DateTimeStyles styles)
            => candidate.ParseDateTimeOrNone();

        [Obsolete("Use " + nameof(ParseTimeSpanOrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        public static Option<TimeSpan> TryParseTimeSpan(this string candidate)
            => candidate.ParseTimeSpanOrNone();

        [Obsolete("Use " + nameof(ParseTimeSpanOrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        public static Option<TimeSpan> TryParseTimeSpan(this string candidate, IFormatProvider provider)
            => candidate.ParseTimeSpanOrNone();

        [Obsolete("Use " + nameof(ParseEnumOrNone) + "<TEnum> instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        public static Option<TEnum> TryParseEnum<TEnum>(this string candidate)
            where TEnum : struct
            => candidate.ParseEnumOrNone<TEnum>();

        [Obsolete("Use " + nameof(ParseEnumOrNone) + "<TEnum> instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        public static Option<TEnum> TryParseEnum<TEnum>(this string candidate, bool ignoreCase)
            where TEnum : struct
            => candidate.ParseEnumOrNone<TEnum>();
    }
}
