#if DATE_ONLY_SUPPORTED
using Funcky.Internal;

namespace Funcky.Extensions;

[OrNoneFromTryPattern(typeof(DateOnly), nameof(DateOnly.TryParse))]
[OrNoneFromTryPattern(typeof(DateOnly), nameof(DateOnly.TryParseExact))]
public static partial class ParseExtensions
{
}
#endif
