using Microsoft.CodeAnalysis;

namespace Funcky.BuiltinAnalyzers;

internal static class SymbolExtensions
{
    public static bool HasAttribute(this ISymbol symbol, INamedTypeSymbol attributeClass)
        => symbol.GetAttributes().Any(attribute => SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, attributeClass));
}
