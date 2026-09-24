#if REFLECTION_ASSEMBLY_NAME_INFO
using System.Reflection.Metadata;
using Funcky.Internal;

namespace Funcky.Extensions;

[OrNoneFromTryPattern(typeof(AssemblyNameInfo), nameof(AssemblyNameInfo.TryParse))]
public static partial class ParseExtensions;
#endif
