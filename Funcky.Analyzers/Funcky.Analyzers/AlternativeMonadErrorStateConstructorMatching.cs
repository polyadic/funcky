using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace Funcky.Analyzers;

internal static class AlternativeMonadErrorStateConstructorMatching
{
    public static bool IsErrorStateConstructorReference(AlternativeMonadType alternativeMonadType, IOperation operation)
        => operation switch
        {
            IPropertyReferenceOperation propertyReference => IsErrorStateConstructorReference(alternativeMonadType, propertyReference),
            IDelegateCreationOperation { Target: IMethodReferenceOperation { Method: var method } } => IsErrorStateConstructorReference(alternativeMonadType, method),
            _ => false,
        };

    private static bool IsErrorStateConstructorReference(AlternativeMonadType alternativeMonadType, IMethodSymbol method)
        => method is { Name: var name, IsStatic: true, ContainingType: var containingType }
           && name == alternativeMonadType.ErrorStateConstructorName
           && IsTypeContainingConstructors(alternativeMonadType, containingType);

    private static bool IsErrorStateConstructorReference(AlternativeMonadType alternativeMonadType, IPropertyReferenceOperation operation)
        => operation is { Property: { Name: var name, IsStatic: true, ContainingType: var containingType } }
           && name == alternativeMonadType.ErrorStateConstructorName
           && IsTypeContainingConstructors(alternativeMonadType, containingType);

    private static bool IsTypeContainingConstructors(AlternativeMonadType alternativeMonadType, INamedTypeSymbol type)
        => SymbolEqualityComparer.Default.Equals(type.ConstructedFrom, alternativeMonadType.Type)
           || SymbolEqualityComparer.Default.Equals(type.ConstructedFrom, alternativeMonadType.ConstructorsType);
}
