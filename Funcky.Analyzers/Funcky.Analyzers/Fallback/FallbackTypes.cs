using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Funcky.Analyzers.Fallback;

/// <summary>The monads whose members come as eager/lazy overload pairs: <c>Option&lt;T&gt;</c>, <c>Result&lt;T&gt;</c> and <c>Either&lt;L, R&gt;</c>.</summary>
internal sealed class FallbackTypes
{
    private readonly ImmutableArray<INamedTypeSymbol> _types;

    private FallbackTypes(ImmutableArray<INamedTypeSymbol> types) => _types = types;

    public bool IsEmpty => _types.IsEmpty;

    public static FallbackTypes FromCompilation(Compilation compilation)
        => new(ImmutableArray.CreateRange(
            new[] { compilation.GetOptionOfTType(), compilation.GetResultOfTType(), compilation.GetEitherOfTType() }
                .Where(type => type is not null)
                .Select(type => type!)));

    public bool Contains(INamedTypeSymbol type)
        => _types.Contains(type.OriginalDefinition, SymbolEqualityComparer.Default);
}
