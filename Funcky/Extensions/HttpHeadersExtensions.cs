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
}
