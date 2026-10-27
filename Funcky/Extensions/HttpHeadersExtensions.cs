using System.Net.Http.Headers;

namespace Funcky.Extensions;

public static partial class HttpHeadersExtensions
{
    extension(HttpHeaders headers)
    {
        public Option<IEnumerable<string>> GetValuesOrNone(string name)
            => headers.TryGetValues(name, out var result)
                ? Option.Some(result)
                : Option<IEnumerable<string>>.None;
    }

    extension(HttpResponseHeaders headers)
    {
        /// <summary>The <c>Location</c> header of the response, or <c>None</c> when the response has none, where
        /// <see cref="HttpResponseHeaders.Location"/> is <see langword="null"/>.</summary>
        public Option<Uri> LocationOrNone
            => Option.FromNullable(headers.Location);
    }
}
