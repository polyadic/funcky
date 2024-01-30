using Funcky.Internal;

namespace Funcky.Extensions;

[OrNoneFromTryPattern(typeof(bool), nameof(bool.TryParse))]
public static partial class ParseExtensions;
