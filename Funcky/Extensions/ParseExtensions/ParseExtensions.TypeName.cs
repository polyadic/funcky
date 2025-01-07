#if REFLECTION_TYPE_NAME
using System.Reflection.Metadata;

namespace Funcky.Extensions;

public static partial class ParseExtensions
{
    [Pure]
    public static Option<TypeName> ParseTypeNameOrNone(
        this ReadOnlySpan<char> candidate,
        TypeNameParseOptions? options = null)
        => TypeName.TryParse(candidate, out var result, options)
            ? result
            : Option<TypeName>.None;
}
#endif
