#if REFLECTION_TYPE_NAME
using System.Reflection.Metadata;
using Funcky.Internal;

namespace Funcky.Extensions;

[OrNoneFromTryPattern(typeof(TypeName), nameof(TypeName.TryParse))]
public static partial class ParseExtensions;
#endif
