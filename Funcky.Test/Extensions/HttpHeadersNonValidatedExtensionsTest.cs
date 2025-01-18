#if HTTP_HEADERS_NON_VALIDATED
using System.Net.Http.Headers;
using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;

namespace Funcky.Test.Extensions;

public sealed class HttpHeadersNonValidatedExtensionsTest
{
    [Property]
    public Property InAnEmptyHttpHeadersNonValidatedGetValueOrNoneIsAlwaysNone(string header)
    {
        var nonValidated = default(HttpHeadersNonValidated);

        return nonValidated
            .GetValuesOrNone(header)
            .Match(none: true, some: False)
            .ToProperty();
    }
}
#endif
