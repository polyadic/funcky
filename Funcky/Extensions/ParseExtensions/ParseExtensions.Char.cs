using Funcky.Internal;

namespace Funcky.Extensions;

[OrNoneFromTryPattern(typeof(char), nameof(char.TryParse))]
public static partial class ParseExtensions;
