﻿//HintName: .g.cs
// <auto-generated/>
#nullable enable

namespace Funcky.Extensions
{
    public static partial class ParseExtensions
    {
        [global::System.Diagnostics.Contracts.Pure]
        public static Funcky.Monads.Option<global::System.DateTime> ParseDateTimeOrNone(this string? candidate) => global::System.DateTime.TryParse(candidate, out var result) ? result : default(Funcky.Monads.Option<global::System.DateTime>);
        [global::System.Diagnostics.Contracts.Pure]
        public static Funcky.Monads.Option<global::System.DateTime> ParseDateTimeOrNone(this global::System.ReadOnlySpan<char> candidate) => global::System.DateTime.TryParse(candidate, out var result) ? result : default(Funcky.Monads.Option<global::System.DateTime>);
        [global::System.Diagnostics.Contracts.Pure]
        public static Funcky.Monads.Option<global::System.DateTime> ParseDateTimeOrNone(this string? candidate, global::System.IFormatProvider? provider, global::System.Globalization.DateTimeStyles styles) => global::System.DateTime.TryParse(candidate, provider, styles, out var result) ? result : default(Funcky.Monads.Option<global::System.DateTime>);
        [global::System.Diagnostics.Contracts.Pure]
        public static Funcky.Monads.Option<global::System.DateTime> ParseDateTimeOrNone(this global::System.ReadOnlySpan<char> candidate, global::System.IFormatProvider? provider, global::System.Globalization.DateTimeStyles styles) => global::System.DateTime.TryParse(candidate, provider, styles, out var result) ? result : default(Funcky.Monads.Option<global::System.DateTime>);
    }
}
