using Microsoft.CodeAnalysis;

namespace Funcky.Analyzers;

internal static class SemanticModelExtensions
{
    public static INamedTypeSymbol NullableOfT(this SemanticModel semanticModel, ITypeSymbol itemType)
        => semanticModel.Compilation.GetSpecialType(SpecialType.System_Nullable_T).Construct(itemType);
}
