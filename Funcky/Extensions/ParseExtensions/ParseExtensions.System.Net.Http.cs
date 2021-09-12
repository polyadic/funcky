using System.Net.Http.Headers;
using Funcky.Internal;

namespace Funcky.Extensions
{
    public static partial class ParseExtensions
    {
        [Pure]
        [OrNoneFromTryPattern(typeof(AuthenticationHeaderValue), nameof(AuthenticationHeaderValue.TryParse))]
        public static partial Option<AuthenticationHeaderValue> ParseAuthenticationHeaderValueOrNone(this string? candidate);

        // implemented manually because CacheControlHeaderValue.TryParse has no annotations to handle nullability correctly.
        [Pure]
        public static Option<CacheControlHeaderValue> ParseCacheControlHeaderValueOrNone(this string? candidate)
            => CacheControlHeaderValue.TryParse(candidate, out var result)
                ? result!
                : Option<CacheControlHeaderValue>.None;

        [Pure]
        [OrNoneFromTryPattern(typeof(ContentDispositionHeaderValue), nameof(ContentDispositionHeaderValue.TryParse))]
        public static partial Option<ContentDispositionHeaderValue> ParseContentDispositionHeaderValueOrNone(this string? candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(ContentRangeHeaderValue), nameof(ContentRangeHeaderValue.TryParse))]
        public static partial Option<ContentRangeHeaderValue> ParseContentRangeHeaderValueOrNone(this string? candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(EntityTagHeaderValue), nameof(EntityTagHeaderValue.TryParse))]
        public static partial Option<EntityTagHeaderValue> ParseEntityTagHeaderValueOrNone(this string? candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(MediaTypeHeaderValue), nameof(MediaTypeHeaderValue.TryParse))]
        public static partial Option<MediaTypeHeaderValue> ParseMediaTypeHeaderValueOrNone(this string? candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(MediaTypeWithQualityHeaderValue), nameof(MediaTypeWithQualityHeaderValue.TryParse))]
        public static partial Option<MediaTypeWithQualityHeaderValue> ParseMediaTypeWithQualityHeaderValueOrNone(this string? candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(NameValueHeaderValue), nameof(NameValueHeaderValue.TryParse))]
        public static partial Option<NameValueHeaderValue> ParseNameValueHeaderValueOrNone(this string? candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(NameValueWithParametersHeaderValue), nameof(NameValueWithParametersHeaderValue.TryParse))]
        public static partial Option<NameValueWithParametersHeaderValue> ParseNameValueWithParametersHeaderValueOrNone(this string? candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(ProductHeaderValue), nameof(ProductHeaderValue.TryParse))]
        public static partial Option<ProductHeaderValue> ParseProductHeaderValueOrNone(this string? candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(ProductInfoHeaderValue), nameof(ProductInfoHeaderValue.TryParse))]
        public static partial Option<ProductInfoHeaderValue> ParseProductInfoHeaderValueOrNone(this string candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(RangeConditionHeaderValue), nameof(RangeConditionHeaderValue.TryParse))]
        public static partial Option<RangeConditionHeaderValue> ParseRangeConditionHeaderValueOrNone(this string? candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(RangeHeaderValue), nameof(RangeHeaderValue.TryParse))]
        public static partial Option<RangeHeaderValue> ParseRangeHeaderValueOrNone(this string? candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(RetryConditionHeaderValue), nameof(RetryConditionHeaderValue.TryParse))]
        public static partial Option<RetryConditionHeaderValue> ParseRetryConditionHeaderValueOrNone(this string? candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(StringWithQualityHeaderValue), nameof(StringWithQualityHeaderValue.TryParse))]
        public static partial Option<StringWithQualityHeaderValue> ParseStringWithQualityHeaderValueOrNone(this string? candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(TransferCodingHeaderValue), nameof(TransferCodingHeaderValue.TryParse))]
        public static partial Option<TransferCodingHeaderValue> ParseTransferCodingHeaderValueOrNone(this string? candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(TransferCodingWithQualityHeaderValue), nameof(TransferCodingWithQualityHeaderValue.TryParse))]
        public static partial Option<TransferCodingWithQualityHeaderValue> ParseTransferCodingWithQualityHeaderValueOrNone(this string? candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(ViaHeaderValue), nameof(ViaHeaderValue.TryParse))]
        public static partial Option<ViaHeaderValue> ParseViaHeaderValueOrNone(this string? candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(WarningHeaderValue), nameof(WarningHeaderValue.TryParse))]
        public static partial Option<WarningHeaderValue> ParseWarningHeaderValueOrNone(this string? candidate);
    }
}
