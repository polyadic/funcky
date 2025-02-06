using Microsoft.CodeAnalysis;

namespace Funcky.Analyzers.CodeAnalysisExtensions;

internal static class SymbolEqualityFunctions
{
    public static bool SymbolEquals(ISymbol? x, ISymbol? y)
        => SymbolEqualityComparer.Default.Equals(x, y);

    public static bool SymbolEqualsIncludeNullability(ISymbol? x, ISymbol? y)
        => SymbolEqualityComparer.IncludeNullability.Equals(x, y);
}
