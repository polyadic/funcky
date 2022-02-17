using System.Net.Http.Headers;
using Funcky.Internal;

namespace Funcky.Extensions
{
    public static partial class ParseExtensions
    {
        [Pure]
        [OrNoneFromTryPattern(typeof(AuthenticationHeaderValue), nameof(AuthenticationHeaderValue.TryParse))]
        public static partial Option<AuthenticationHeaderValue> TryParseAuthenticationHeaderValueOrNone(string? candidate);

        // implemented manually because CacheControlHeaderValue.TryParse has no annotations to handle nullability correctly.
        [Pure]
        public static Option<CacheControlHeaderValue> TryParseCacheControlHeaderValueOrNone(string? candidate)
            => CacheControlHeaderValue.TryParse(candidate, out var result)
                ? result!
                : Option<CacheControlHeaderValue>.None();

        [Pure]
        [OrNoneFromTryPattern(typeof(ContentDispositionHeaderValue), nameof(ContentDispositionHeaderValue.TryParse))]
        public static partial Option<ContentDispositionHeaderValue> TryParseContentDispositionHeaderValueOrNone(string? candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(ContentRangeHeaderValue), nameof(ContentRangeHeaderValue.TryParse))]
        public static partial Option<ContentRangeHeaderValue> TryParseContentRangeHeaderValueOrNone(string? candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(EntityTagHeaderValue), nameof(EntityTagHeaderValue.TryParse))]
        public static partial Option<EntityTagHeaderValue> TryParseEntityTagHeaderValueOrNone(string? candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(MediaTypeHeaderValue), nameof(MediaTypeHeaderValue.TryParse))]
        public static partial Option<MediaTypeHeaderValue> TryParseMediaTypeHeaderValueOrNone(string? candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(MediaTypeWithQualityHeaderValue), nameof(MediaTypeWithQualityHeaderValue.TryParse))]
        public static partial Option<MediaTypeWithQualityHeaderValue> TryParseMediaTypeWithQualityHeaderValueOrNone(string? candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(NameValueHeaderValue), nameof(NameValueHeaderValue.TryParse))]
        public static partial Option<NameValueHeaderValue> TryParseNameValueHeaderValueOrNone(string? candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(NameValueWithParametersHeaderValue), nameof(NameValueWithParametersHeaderValue.TryParse))]
        public static partial Option<NameValueWithParametersHeaderValue> TryParseNameValueWithParametersHeaderValueOrNone(string? candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(ProductHeaderValue), nameof(ProductHeaderValue.TryParse))]
        public static partial Option<ProductHeaderValue> TryParseProductHeaderValueOrNone(string? candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(ProductInfoHeaderValue), nameof(ProductInfoHeaderValue.TryParse))]
        public static partial Option<ProductInfoHeaderValue> TryParseProductInfoHeaderValueOrNone(string candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(RangeConditionHeaderValue), nameof(RangeConditionHeaderValue.TryParse))]
        public static partial Option<RangeConditionHeaderValue> TryParseRangeConditionHeaderValueOrNone(string? candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(RangeHeaderValue), nameof(RangeHeaderValue.TryParse))]
        public static partial Option<RangeHeaderValue> TryParseRangeHeaderValueOrNone(string? candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(RetryConditionHeaderValue), nameof(RetryConditionHeaderValue.TryParse))]
        public static partial Option<RetryConditionHeaderValue> TryParseRetryConditionHeaderValueOrNone(string? candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(StringWithQualityHeaderValue), nameof(StringWithQualityHeaderValue.TryParse))]
        public static partial Option<StringWithQualityHeaderValue> TryParseStringWithQualityHeaderValueOrNone(string? candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(TransferCodingHeaderValue), nameof(TransferCodingHeaderValue.TryParse))]
        public static partial Option<TransferCodingHeaderValue> TryParseTransferCodingHeaderValueOrNone(string? candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(TransferCodingWithQualityHeaderValue), nameof(TransferCodingWithQualityHeaderValue.TryParse))]
        public static partial Option<TransferCodingWithQualityHeaderValue> TryParseTransferCodingWithQualityHeaderValueOrNone(string? candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(ViaHeaderValue), nameof(ViaHeaderValue.TryParse))]
        public static partial Option<ViaHeaderValue> TryParseViaHeaderValueOrNone(string? candidate);

        [Pure]
        [OrNoneFromTryPattern(typeof(WarningHeaderValue), nameof(WarningHeaderValue.TryParse))]
        public static partial Option<WarningHeaderValue> TryParseWarningHeaderValueOrNone(string? candidate);
    }
}
