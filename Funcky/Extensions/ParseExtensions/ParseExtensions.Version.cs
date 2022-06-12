using Funcky.Internal;

namespace Funcky.Extensions;

[OrNoneFromTryPattern(typeof(Version), nameof(Version.TryParse))]
public static partial class ParseExtensions
{
}
