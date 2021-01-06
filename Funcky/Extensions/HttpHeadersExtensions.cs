using System.Collections.Generic;
using System.Net.Http.Headers;
using Funcky.Monads;

namespace Funcky.Extensions
{
    public static partial class HttpHeadersExtensions
    {
        public static Option<IEnumerable<string>> GetValuesOrNone(this HttpHeaders headers, string name)
            => headers.TryGetValues(name, out var values)
                   ? Option.Some(values)
                   : Option<IEnumerable<string>>.None();
    }
}
