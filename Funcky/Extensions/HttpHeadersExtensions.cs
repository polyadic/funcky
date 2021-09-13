using System.Net.Http.Headers;
using Funcky.Internal;

namespace Funcky.Extensions
{
    public static partial class HttpHeadersExtensions
    {
        public static Option<IEnumerable<string>> GetValuesOrNone(this HttpHeaders headers, string name)
            => FailToOption<IEnumerable<string>>.FromTryPatternHandleNull(headers.TryGetValues, name);
    }
}
