using System.Net.Http.Headers;

namespace Funcky.Extensions;

/// <summary>The static form of <see cref="ParseExtensions.ParseCacheControlHeaderValueOrNone(string?)"/>: <c>CacheControlHeaderValue.ParseOrNone(text)</c>.</summary>
public static class CacheControlHeaderValueParseExtensions
{
    extension(CacheControlHeaderValue)
    {
        /// <summary>Calls <see cref="CacheControlHeaderValue.TryParse"/> and returns the parsed value, or None when it fails.</summary>
        // implemented manually because CacheControlHeaderValue.TryParse has no annotations to handle nullability correctly.
        [Pure]
        public static Option<CacheControlHeaderValue> ParseOrNone(string? candidate)
            => CacheControlHeaderValue.TryParse(candidate, out var result)
                ? result ?? new CacheControlHeaderValue()
                : Option<CacheControlHeaderValue>.None;
    }
}
