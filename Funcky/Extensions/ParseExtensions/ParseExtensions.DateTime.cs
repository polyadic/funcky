using Funcky.Internal;

namespace Funcky.Extensions;

[OrNoneFromTryPattern(typeof(DateTime), nameof(DateTime.TryParse))]
[OrNoneFromTryPattern(typeof(DateTime), nameof(DateTime.TryParseExact))]
public static partial class ParseExtensions;
