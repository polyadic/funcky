#if HTTP_HEADERS_NON_VALIDATED
using System.Net.Http.Headers;

namespace Funcky.Extensions;

public static class HttpHeadersNonValidatedExtensions
{
    extension(HttpHeadersNonValidated headers)
    {
        public Option<HeaderStringValues> GetValuesOrNone(string headerName)
            => headers.TryGetValues(headerName, out var values)
                ? values
                : Option<HeaderStringValues>.None;
    }
}
#endif
