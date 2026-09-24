using Microsoft.CodeAnalysis;

namespace Funcky.Analyzers;

public static class FunckyWellKnownTypeNames
{
    public static INamedTypeSymbol? GetEnumerableType(this Compilation compilation) => compilation.GetTypeByMetadataName("System.Linq.Enumerable");

    public static INamedTypeSymbol? GetEnumerableExtensionType(this Compilation compilation) => compilation.GetTypeByMetadataName("Funcky.Extensions.EnumerableExtensions");

    public static INamedTypeSymbol? GetOptionOfTType(this Compilation compilation) => compilation.GetTypeByMetadataName("Funcky.Monads.Option`1");

    public static INamedTypeSymbol? GetOptionType(this Compilation compilation) => compilation.GetTypeByMetadataName("Funcky.Monads.Option");

    public static INamedTypeSymbol? GetOptionExtensionsType(this Compilation compilation) => compilation.GetTypeByMetadataName("Funcky.Monads.OptionExtensions");

    public static INamedTypeSymbol? GetEitherType(this Compilation compilation) => compilation.GetTypeByMetadataName("Funcky.Monads.Either`1");

    public static INamedTypeSymbol? GetEitherOfTType(this Compilation compilation) => compilation.GetTypeByMetadataName("Funcky.Monads.Either`2");

    public static INamedTypeSymbol? GetResultType(this Compilation compilation) => compilation.GetTypeByMetadataName("Funcky.Monads.Result");

    public static INamedTypeSymbol? GetResultOfTType(this Compilation compilation) => compilation.GetTypeByMetadataName("Funcky.Monads.Result`1");

    public static INamedTypeSymbol? GetSequenceType(this Compilation compilation) => compilation.GetTypeByMetadataName("Funcky.Sequence");

    public static INamedTypeSymbol? GetFunctionalType(this Compilation compilation) => compilation.GetTypeByMetadataName("Funcky.Functional");

    public static INamedTypeSymbol? GetExpressionOfTType(this Compilation compilation) => compilation.GetTypeByMetadataName("System.Linq.Expressions.Expression`1");

    public static INamedTypeSymbol? GetXunitAssertType(this Compilation compilation) => compilation.GetTypeByMetadataName("Xunit.Assert");
}
