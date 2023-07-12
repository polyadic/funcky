using Microsoft.CodeAnalysis;
using static Funcky.Analyzers.FunckyWellKnownMemberNames;

namespace Funcky.Analyzers;

internal sealed class AlternativeMonadType
{
    private readonly bool _matchHasSuccessStateFirst;
    private readonly string _returnAlias;
    private readonly Lazy<bool> _hasGetOrElse;

    private AlternativeMonadType(INamedTypeSymbol type, bool matchHasSuccessStateFirst, string returnAlias)
    {
        _matchHasSuccessStateFirst = matchHasSuccessStateFirst;
        _returnAlias = returnAlias;
        _hasGetOrElse = new(() => type.GetMembers().Any(static member => member is IMethodSymbol { Name: GetOrElseMethodName }));
    }

    public int ErrorStateArgumentIndex => _matchHasSuccessStateFirst ? 1 : 0;

    public int SuccessStateArgumentIndex => _matchHasSuccessStateFirst ? 0 : 1;

    public bool HasGetOrElse => _hasGetOrElse.Value;

    public static AlternativeMonadType? Create(INamedTypeSymbol type, INamedTypeSymbol alternativeMonadAttributeType)
        => type.GetAttributes()
            .Where(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, alternativeMonadAttributeType))
            .Select(Create(type.ConstructedFrom))
            .SingleOrDefault();

    private static Func<AttributeData, AlternativeMonadType> Create(INamedTypeSymbol type)
        => attribute => new(
            type: type,
            matchHasSuccessStateFirst: (bool)(attribute.NamedArguments.SingleOrDefault(arg => arg.Key == "MatchHasSuccessStateFirst").Value.Value ?? false),
            returnAlias: (string)(attribute.NamedArguments.Single(arg => arg.Key == "ReturnAlias").Value.Value ?? throw new InvalidOperationException()));
}
