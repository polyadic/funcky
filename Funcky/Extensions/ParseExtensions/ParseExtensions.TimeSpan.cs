using Funcky.Internal;

namespace Funcky.Extensions;

[OrNoneFromTryPattern(typeof(TimeSpan), nameof(TimeSpan.TryParse))]
[OrNoneFromTryPattern(typeof(TimeSpan), nameof(TimeSpan.TryParseExact))]
public static partial class ParseExtensions;
