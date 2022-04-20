using Microsoft.CodeAnalysis;

namespace Funcky.BuiltinAnalyzers;

internal static class CompilationExtensions
{
    public static INamedTypeSymbol? GetOptionOfTType(this Compilation compilation) => compilation.GetTypeByMetadataName("Funcky.Monads.Option`1");

    public static INamedTypeSymbol? GetOptionType(this Compilation compilation) => compilation.GetTypeByMetadataName("Funcky.Monads.Option");
}
