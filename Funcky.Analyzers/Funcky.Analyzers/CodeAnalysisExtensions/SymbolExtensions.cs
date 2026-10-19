using Microsoft.CodeAnalysis;

namespace Funcky.Analyzers.CodeAnalysisExtensions;

internal static class SymbolExtensions
{
    public static bool HasAttribute(this ISymbol symbol, INamedTypeSymbol attributeClass)
        => symbol.GetAttributes().Any(attribute => SymbolEquals(attribute.AttributeClass, attributeClass));

    /// <summary>Matches the attribute by its fully qualified name rather than by symbol, so that a copy of the attribute
    /// in any assembly counts: the one in Funcky, one pasted into the user's project or one emitted by the generator.
    /// (<c>GetTypeByMetadataName</c> returns <see langword="null"/> as soon as two assemblies define the same name.)</summary>
    public static bool HasAttribute(this ISymbol symbol, string attributeFullName)
        => symbol.GetAttributes().Any(attribute => attribute.AttributeClass is { } attributeClass && IsNamed(attributeClass, attributeFullName));

    private static bool IsNamed(INamedTypeSymbol type, string fullName)
        => type is { ContainingType: null, IsGenericType: false }
            && fullName.EndsWith(type.Name, StringComparison.Ordinal)
            && IsNamespace(type.ContainingNamespace, fullName, fullName.Length - type.Name.Length);

    /// <summary>Checks that the first <paramref name="length"/> characters of <paramref name="fullName"/> spell the namespace followed by a dot.</summary>
    private static bool IsNamespace(INamespaceSymbol @namespace, string fullName, int length)
        => @namespace.IsGlobalNamespace
            ? length == 0
            : length > @namespace.Name.Length
                && fullName[length - 1] == '.'
                && string.CompareOrdinal(fullName, length - 1 - @namespace.Name.Length, @namespace.Name, 0, @namespace.Name.Length) == 0
                && IsNamespace(@namespace.ContainingNamespace, fullName, length - 1 - @namespace.Name.Length);
}
