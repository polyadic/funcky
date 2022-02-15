﻿//HintName: ParseExtensions.g.cs
// <auto-generated/>
#nullable enable

using Funcky.Internal;

namespace Funcky.Extensions
{
    public static partial class ParseExtensions
    {
        [Pure]
        public static partial Option<bool> ParseBooleanOrNone(this string candidate) => FailToOption<bool>.FromTryPattern(bool.TryParse, candidate);
        [Pure]
        public static partial Option<DateTime> ParseDateTimeOrNone(this string candidate, IFormatProvider provider, DateTimeStyles styles) => FailToOption<DateTime>.FromTryPattern(DateTime.TryParse, candidate, provider, styles);
    }
}