using Microsoft.CodeAnalysis;
using static Funcky.Analyzers.FunckyWellKnownMemberNames;

namespace Funcky.Analyzers.AlternativeMonad;

internal sealed class AlternativeMonadType(
    INamedTypeSymbol type,
    INamedTypeSymbol constructorsType,
    string returnAlias,
    string errorStateConstructorName,
    string errorStateParameterName,
    INamedTypeSymbol? extensionsType = null)
{
    private readonly Lazy<bool> _hasGetOrElse = HasMethodLazy(type, GetOrElseMethodName);
    private readonly Lazy<bool> _hasOrElse = HasMethodLazy(type, OrElseMethodName);
    private readonly Lazy<bool> _hasToNullable = new(() => extensionsType is not null && HasMethod(extensionsType, ToNullableMethodName));

    public INamedTypeSymbol Type { get; } = type;

    public INamedTypeSymbol ConstructorsType { get; } = constructorsType;

    public string ReturnAlias { get; } = returnAlias;

    public string ErrorStateConstructorName { get; } = errorStateConstructorName;

    /// <summary>The name of the <c>Match</c> parameter that handles the error state (<c>none</c>, <c>left</c>, <c>error</c>). The
    /// arguments are told apart by name rather than by position, so the analyzer works with Funcky 3.x, where <c>Result.Match</c>
    /// took the success state first, and with Funcky 4.0.0, where the error state comes first everywhere.</summary>
    public string ErrorStateParameterName { get; } = errorStateParameterName;

    public bool HasGetOrElse => _hasGetOrElse.Value;

    public bool HasOrElse => _hasOrElse.Value;

    public bool HasToNullable => _hasToNullable.Value;

    private static Lazy<bool> HasMethodLazy(INamedTypeSymbol type, string expectedName)
        => new(() => HasMethod(type, expectedName));

    private static bool HasMethod(INamedTypeSymbol type, string expectedName)
        => type.GetMembers().Any(member => member is IMethodSymbol { Name: var name } && name == expectedName);
}
