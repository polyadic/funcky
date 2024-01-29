using Funcky.Internal;

namespace Funcky.Extensions;

[OrNoneFromTryPattern(typeof(Guid), nameof(Guid.TryParse))]
[OrNoneFromTryPattern(typeof(Guid), nameof(Guid.TryParseExact))]
public static partial class ParseExtensions;
