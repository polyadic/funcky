using Microsoft.CodeAnalysis;

namespace Funcky.Analyzers;

internal static class SymbolExtensions
{
    public static bool HasAttribute(this ISymbol symbol, INamedTypeSymbol attributeClass)
        => symbol.GetAttributes().Any(attribute => SymbolEquals(attribute.AttributeClass, attributeClass));
}
