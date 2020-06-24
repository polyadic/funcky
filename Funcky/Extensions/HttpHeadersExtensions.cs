using System.Collections.Generic;
using System.Net.Http.Headers;
using Funcky.Monads;

namespace Funcky.Extensions
{
    public static class HttpHeadersExtensions
    {
        public static Option<IEnumerable<string>> TryGetValues(this HttpHeaders headers, string name)
            => headers.TryGetValues(name, out var values)
                   ? Option.Some(values)
                   : Option<IEnumerable<string>>.None();
    }
}
