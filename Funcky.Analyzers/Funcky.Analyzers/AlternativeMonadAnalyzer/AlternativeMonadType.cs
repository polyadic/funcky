using Microsoft.CodeAnalysis;
using static Funcky.Analyzers.FunckyWellKnownMemberNames;

namespace Funcky.Analyzers;

internal sealed class AlternativeMonadType(
    INamedTypeSymbol type,
    bool matchHasSuccessStateFirst,
    string returnAlias,
    INamedTypeSymbol? extensionsType = null)
{
    private readonly Lazy<bool> _hasGetOrElse = HasMethodLazy(type, GetOrElseMethodName);
    private readonly Lazy<bool> _hasOrElse = HasMethodLazy(type, OrElseMethodName);
    private readonly Lazy<bool> _hasToNullable = new(() => extensionsType is not null && HasMethod(extensionsType, OptionToNullableMethodName));

    public INamedTypeSymbol Type { get; } = type;

    public int ErrorStateArgumentIndex => matchHasSuccessStateFirst ? 1 : 0;

    public int SuccessStateArgumentIndex => matchHasSuccessStateFirst ? 0 : 1;

    public bool HasGetOrElse => _hasGetOrElse.Value;

    public bool HasOrElse => _hasOrElse.Value;

    public bool HasToNullable => _hasToNullable.Value;

    private static Lazy<bool> HasMethodLazy(INamedTypeSymbol type, string expectedName)
        => new(() => HasMethod(type, expectedName));

    private static bool HasMethod(INamedTypeSymbol type, string expectedName)
        => type.GetMembers().Any(member => member is IMethodSymbol { Name: var name } && name == expectedName);
}
