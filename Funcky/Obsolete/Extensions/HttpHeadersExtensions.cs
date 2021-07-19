using System.ComponentModel;
using System.Net.Http.Headers;

namespace Funcky.Extensions
{
    public static partial class HttpHeadersExtensions
    {
        [Obsolete("Use " + nameof(GetValuesOrNone) + " instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Option<IEnumerable<string>> TryGetValues(this HttpHeaders headers, string name)
            => headers.GetValuesOrNone(name);
    }
}
