using System.Net.Http.Headers;
using Funcky.Internal;

namespace Funcky.Extensions;

[OrNoneFromTryPattern(typeof(AuthenticationHeaderValue), nameof(AuthenticationHeaderValue.TryParse))]
[OrNoneFromTryPattern(typeof(ContentDispositionHeaderValue), nameof(ContentDispositionHeaderValue.TryParse))]
[OrNoneFromTryPattern(typeof(ContentRangeHeaderValue), nameof(ContentRangeHeaderValue.TryParse))]
[OrNoneFromTryPattern(typeof(EntityTagHeaderValue), nameof(EntityTagHeaderValue.TryParse))]
[OrNoneFromTryPattern(typeof(MediaTypeHeaderValue), nameof(MediaTypeHeaderValue.TryParse))]
[OrNoneFromTryPattern(typeof(MediaTypeWithQualityHeaderValue), nameof(MediaTypeWithQualityHeaderValue.TryParse))]
[OrNoneFromTryPattern(typeof(NameValueHeaderValue), nameof(NameValueHeaderValue.TryParse))]
[OrNoneFromTryPattern(typeof(NameValueWithParametersHeaderValue), nameof(NameValueWithParametersHeaderValue.TryParse))]
[OrNoneFromTryPattern(typeof(ProductHeaderValue), nameof(ProductHeaderValue.TryParse))]
[OrNoneFromTryPattern(typeof(ProductInfoHeaderValue), nameof(ProductInfoHeaderValue.TryParse))]
[OrNoneFromTryPattern(typeof(RangeConditionHeaderValue), nameof(RangeConditionHeaderValue.TryParse))]
[OrNoneFromTryPattern(typeof(RangeHeaderValue), nameof(RangeHeaderValue.TryParse))]
[OrNoneFromTryPattern(typeof(RetryConditionHeaderValue), nameof(RetryConditionHeaderValue.TryParse))]
[OrNoneFromTryPattern(typeof(StringWithQualityHeaderValue), nameof(StringWithQualityHeaderValue.TryParse))]
[OrNoneFromTryPattern(typeof(TransferCodingHeaderValue), nameof(TransferCodingHeaderValue.TryParse))]
[OrNoneFromTryPattern(typeof(TransferCodingWithQualityHeaderValue), nameof(TransferCodingWithQualityHeaderValue.TryParse))]
[OrNoneFromTryPattern(typeof(ViaHeaderValue), nameof(ViaHeaderValue.TryParse))]
[OrNoneFromTryPattern(typeof(WarningHeaderValue), nameof(WarningHeaderValue.TryParse))]
public static partial class ParseExtensions
{
    // implemented manually because CacheControlHeaderValue.TryParse has no annotations to handle nullability correctly.
    [Pure]
    public static Option<CacheControlHeaderValue> ParseCacheControlHeaderValueOrNone(this string? candidate)
        => CacheControlHeaderValue.TryParse(candidate, out var result)
            ? result ?? new CacheControlHeaderValue()
            : Option<CacheControlHeaderValue>.None;
}
