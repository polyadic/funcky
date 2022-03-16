using Microsoft.CodeAnalysis;

namespace Funcky.Analyzers;

public static class CompilationExtensions
{
    public static INamedTypeSymbol? GetEnumerableType(this Compilation compilation) => compilation.GetTypeByMetadataName("System.Linq.Enumerable");

    public static INamedTypeSymbol? GetStringType(this Compilation compilation) => compilation.GetTypeByMetadataName("System.String");

    public static INamedTypeSymbol? GetEnumerableExtensionType(this Compilation compilation) => compilation.GetTypeByMetadataName("Funcky.Extensions.EnumerableExtensions");

    public static INamedTypeSymbol? GetOptionOfTType(this Compilation compilation) => compilation.GetTypeByMetadataName("Funcky.Monads.Option`1");

    public static INamedTypeSymbol? GetOptionType(this Compilation compilation) => compilation.GetTypeByMetadataName("Funcky.Monads.Option");

    public static INamedTypeSymbol? GetSequenceType(this Compilation compilation) => compilation.GetTypeByMetadataName("Funcky.Sequence");
}
