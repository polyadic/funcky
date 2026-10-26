using System.Net.Http.Headers;

namespace Funcky.Extensions;

/// <summary>The static form of <see cref="ParseExtensions.ParseCacheControlHeaderValueOrNone(string?)"/>: <c>CacheControlHeaderValue.ParseOrNone(text)</c>.</summary>
public static class CacheControlHeaderValueParseExtensions
{
    extension(CacheControlHeaderValue)
    {
        /// <summary>Calls <see cref="CacheControlHeaderValue.TryParse"/> and returns the parsed value, or None when it fails.</summary>
        [Pure]
        public static Option<CacheControlHeaderValue> ParseOrNone(string? candidate)
            => candidate.ParseCacheControlHeaderValueOrNone();
    }
}
