using Microsoft.CodeAnalysis;
using static Funcky.Analyzers.FunckyWellKnownMemberNames;

namespace Funcky.Analyzers;

internal sealed class AlternativeMonadType
{
    private readonly bool _matchHasSuccessStateFirst;
    private readonly string _returnAlias;
    private readonly Lazy<bool> _hasGetOrElse;

    public AlternativeMonadType(INamedTypeSymbol type, bool matchHasSuccessStateFirst, string returnAlias)
    {
        Type = type;
        _matchHasSuccessStateFirst = matchHasSuccessStateFirst;
        _returnAlias = returnAlias;
        _hasGetOrElse = new(() => type.GetMembers().Any(static member => member is IMethodSymbol { Name: GetOrElseMethodName }));
    }

    public INamedTypeSymbol Type { get; }

    public int ErrorStateArgumentIndex => _matchHasSuccessStateFirst ? 1 : 0;

    public int SuccessStateArgumentIndex => _matchHasSuccessStateFirst ? 0 : 1;

    public bool HasGetOrElse => _hasGetOrElse.Value;
}
