using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Funcky.BuiltinAnalyzers;

internal static class CompilationExtensions
{
    public static INamedTypeSymbol? GetOptionOfTType(this Compilation compilation) => compilation.GetTypeByMetadataName("Funcky.Monads.Option`1");

    public static INamedTypeSymbol? GetOptionType(this Compilation compilation) => compilation.GetTypeByMetadataName("Funcky.Monads.Option");

    public static INamedTypeSymbol? GetResultOfTType(this Compilation compilation) => compilation.GetTypeByMetadataName("Funcky.Monads.Result`1");

    public static INamedTypeSymbol? GetEitherOfTType(this Compilation compilation) => compilation.GetTypeByMetadataName("Funcky.Monads.Either`2");

    /// <summary>The monads that define an implicit conversion from their inner type: <c>Option&lt;T&gt;</c> from <c>T</c>,
    /// <c>Result&lt;T&gt;</c> from <c>T</c> and <c>Either&lt;L, R&gt;</c> from <c>R</c>.</summary>
    public static ImmutableArray<INamedTypeSymbol> GetImplicitlyConvertibleMonadTypes(this Compilation compilation)
        => ImmutableArray.CreateRange(
            new[] { compilation.GetOptionOfTType(), compilation.GetResultOfTType(), compilation.GetEitherOfTType() }
                .Where(type => type is not null)
                .Select(type => type!));

    public static INamedTypeSymbol? GetResultType(this Compilation compilation) => compilation.GetTypeByMetadataName("Funcky.Monads.Result");

    public static INamedTypeSymbol? GetEitherOfLeftType(this Compilation compilation) => compilation.GetTypeByMetadataName("Funcky.Monads.Either`1");
}
