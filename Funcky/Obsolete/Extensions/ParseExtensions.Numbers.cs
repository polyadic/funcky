using System.ComponentModel;
using System.Globalization;
using Funcky.Internal;

namespace Funcky.Extensions
{
    public static partial class ParseExtensions
    {
        [Obsolete("Use " + nameof(ParseInt16OrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        [OrNoneFromTryPattern(typeof(short), nameof(short.TryParse))]
        public static partial Option<short> ParseShortOrNone(this string candidate);

        [Obsolete("Use " + nameof(ParseInt16OrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        [OrNoneFromTryPattern(typeof(short), nameof(short.TryParse))]
        public static partial Option<short> ParseShortOrNone(this string candidate, NumberStyles styles, IFormatProvider provider);

        [Obsolete("Use " + nameof(ParseUInt16OrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        [OrNoneFromTryPattern(typeof(ushort), nameof(ushort.TryParse))]
        public static partial Option<ushort> ParseUShortOrNone(this string candidate, NumberStyles styles, IFormatProvider provider);

        [Obsolete("Use " + nameof(ParseUInt16OrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        [OrNoneFromTryPattern(typeof(ushort), nameof(ushort.TryParse))]
        public static partial Option<ushort> ParseUShortOrNone(this string candidate);

        [Obsolete("Use " + nameof(ParseInt32OrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        [OrNoneFromTryPattern(typeof(int), nameof(int.TryParse))]
        public static partial Option<int> ParseIntOrNone(this string candidate);

        [Obsolete("Use " + nameof(ParseUInt32OrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        [OrNoneFromTryPattern(typeof(uint), nameof(uint.TryParse))]
        public static partial Option<uint> ParseUIntOrNone(this string candidate, NumberStyles styles, IFormatProvider provider);

        [Obsolete("Use " + nameof(ParseUInt32OrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        [OrNoneFromTryPattern(typeof(uint), nameof(uint.TryParse))]
        public static partial Option<uint> ParseUIntOrNone(this string candidate);

        [Obsolete("Use " + nameof(ParseInt32OrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        [OrNoneFromTryPattern(typeof(int), nameof(int.TryParse))]
        public static partial Option<int> ParseIntOrNone(this string candidate, NumberStyles styles, IFormatProvider provider);

        [Obsolete("Use " + nameof(ParseInt64OrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        [OrNoneFromTryPattern(typeof(long), nameof(long.TryParse))]
        public static partial Option<long> ParseLongOrNone(this string candidate);

        [Obsolete("Use " + nameof(ParseInt64OrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        [OrNoneFromTryPattern(typeof(long), nameof(long.TryParse))]
        public static partial Option<long> ParseLongOrNone(this string candidate, NumberStyles styles, IFormatProvider provider);

        [Obsolete("Use " + nameof(ParseUInt64OrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        [OrNoneFromTryPattern(typeof(ulong), nameof(ulong.TryParse))]
        public static partial Option<ulong> ParseULongOrNone(this string candidate);

        [Obsolete("Use " + nameof(ParseUInt64OrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        [OrNoneFromTryPattern(typeof(ulong), nameof(ulong.TryParse))]
        public static partial Option<ulong> ParseULongOrNone(this string candidate, NumberStyles styles, IFormatProvider provider);

        [Obsolete("Use " + nameof(ParseSingleOrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        [OrNoneFromTryPattern(typeof(float), nameof(float.TryParse))]
        public static partial Option<float> ParseFloatOrNone(this string candidate);

        [Obsolete("Use " + nameof(ParseSingleOrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        [OrNoneFromTryPattern(typeof(float), nameof(float.TryParse))]
        public static partial Option<float> ParseFloatOrNone(this string candidate, NumberStyles styles, IFormatProvider provider);

#if READ_ONLY_SPAN_SUPPORTED
        [Obsolete("Use " + nameof(ParseInt16OrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        [OrNoneFromTryPattern(typeof(short), nameof(short.TryParse))]
        public static partial Option<short> ParseShortOrNone(this ReadOnlySpan<char> candidate);

        [Obsolete("Use " + nameof(ParseInt16OrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        [OrNoneFromTryPattern(typeof(short), nameof(short.TryParse))]
        public static partial Option<short> ParseShortOrNone(this ReadOnlySpan<char> candidate, NumberStyles styles, IFormatProvider provider);

        [Obsolete("Use " + nameof(ParseUInt16OrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        [OrNoneFromTryPattern(typeof(ushort), nameof(ushort.TryParse))]
        public static partial Option<ushort> ParseUShortOrNone(this ReadOnlySpan<char> candidate, NumberStyles styles, IFormatProvider provider);

        [Obsolete("Use " + nameof(ParseUInt16OrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        [OrNoneFromTryPattern(typeof(ushort), nameof(ushort.TryParse))]
        public static partial Option<ushort> ParseUShortOrNone(this ReadOnlySpan<char> candidate);

        [Obsolete("Use " + nameof(ParseInt32OrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        [OrNoneFromTryPattern(typeof(int), nameof(int.TryParse))]
        public static partial Option<int> ParseIntOrNone(this ReadOnlySpan<char> candidate);

        [Obsolete("Use " + nameof(ParseUInt32OrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        [OrNoneFromTryPattern(typeof(uint), nameof(uint.TryParse))]
        public static partial Option<uint> ParseUIntOrNone(this ReadOnlySpan<char> candidate, NumberStyles styles, IFormatProvider provider);

        [Obsolete("Use " + nameof(ParseUInt32OrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        [OrNoneFromTryPattern(typeof(uint), nameof(uint.TryParse))]
        public static partial Option<uint> ParseUIntOrNone(this ReadOnlySpan<char> candidate);

        [Obsolete("Use " + nameof(ParseInt32OrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        [OrNoneFromTryPattern(typeof(int), nameof(int.TryParse))]
        public static partial Option<int> ParseIntOrNone(this ReadOnlySpan<char> candidate, NumberStyles styles, IFormatProvider provider);

        [Obsolete("Use " + nameof(ParseInt64OrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        [OrNoneFromTryPattern(typeof(long), nameof(long.TryParse))]
        public static partial Option<long> ParseLongOrNone(this ReadOnlySpan<char> candidate);

        [Obsolete("Use " + nameof(ParseInt64OrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        [OrNoneFromTryPattern(typeof(long), nameof(long.TryParse))]
        public static partial Option<long> ParseLongOrNone(this ReadOnlySpan<char> candidate, NumberStyles styles, IFormatProvider provider);

        [Obsolete("Use " + nameof(ParseUInt64OrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        [OrNoneFromTryPattern(typeof(ulong), nameof(ulong.TryParse))]
        public static partial Option<ulong> ParseULongOrNone(this ReadOnlySpan<char> candidate);

        [Obsolete("Use " + nameof(ParseUInt64OrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        [OrNoneFromTryPattern(typeof(ulong), nameof(ulong.TryParse))]
        public static partial Option<ulong> ParseULongOrNone(this ReadOnlySpan<char> candidate, NumberStyles styles, IFormatProvider provider);

        [Obsolete("Use " + nameof(ParseSingleOrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        [OrNoneFromTryPattern(typeof(float), nameof(float.TryParse))]
        public static partial Option<float> ParseFloatOrNone(this ReadOnlySpan<char> candidate);

        [Obsolete("Use " + nameof(ParseSingleOrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        [OrNoneFromTryPattern(typeof(float), nameof(float.TryParse))]
        public static partial Option<float> ParseFloatOrNone(this ReadOnlySpan<char> candidate, NumberStyles styles, IFormatProvider provider);
#endif
    }
}
