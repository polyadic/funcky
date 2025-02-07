using Microsoft.CodeAnalysis;

namespace Funcky.Analyzers.CodeAnalysisExtensions;

internal static class SemanticModelExtensions
{
    public static INamedTypeSymbol NullableOfT(this SemanticModel semanticModel, ITypeSymbol itemType)
        => semanticModel.Compilation.GetSpecialType(SpecialType.System_Nullable_T).Construct(itemType);
}
