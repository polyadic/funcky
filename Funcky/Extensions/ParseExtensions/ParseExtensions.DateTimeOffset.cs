using Funcky.Internal;

namespace Funcky.Extensions;

[OrNoneFromTryPattern(typeof(DateTimeOffset), nameof(DateTimeOffset.TryParse))]
[OrNoneFromTryPattern(typeof(DateTimeOffset), nameof(DateTimeOffset.TryParseExact))]
public static partial class ParseExtensions;
