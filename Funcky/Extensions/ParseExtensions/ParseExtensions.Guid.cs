using Funcky.Internal;

namespace Funcky.Extensions;

[OrNoneFromTryPattern(typeof(Guid), nameof(Guid.TryParse))]
[OrNoneFromTryPattern(typeof(Guid), nameof(Guid.TryParseExact))]
public partial class ParseExtensions
{
}
