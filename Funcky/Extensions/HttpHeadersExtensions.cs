using System.Net.Http.Headers;

namespace Funcky.Extensions
{
    public static partial class HttpHeadersExtensions
    {
        public static Option<IEnumerable<string>> GetValuesOrNone(this HttpHeaders headers, string name)
            => headers.TryGetValues(name, out IEnumerable<string>? result)
                ? Option.FromNullable(result)
                : Option<IEnumerable<string>>.None();
    }
}
