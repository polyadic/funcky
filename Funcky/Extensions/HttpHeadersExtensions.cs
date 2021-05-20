using System.Net.Http.Headers;

namespace Funcky.Extensions
{
    public static partial class HttpHeadersExtensions
    {
        public static Option<IEnumerable<string>> GetValuesOrNone(this HttpHeaders headers, string name)
            => headers.TryGetValues(name, out var result)
                ? Option.Some(result)
                : Option<IEnumerable<string>>.None;
    }
}
