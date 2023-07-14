using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using static Funcky.Analyzers.FunckyWellKnownMemberNames;

namespace Funcky.Analyzers;

public partial class AlternativeMonadAnalyzer
{
    public static readonly DiagnosticDescriptor PreferSelectMany = new DiagnosticDescriptor(
        id: $"{DiagnosticName.Prefix}{DiagnosticName.Usage}07",
        title: $"Prefer {SelectManyMethodName} over {MatchMethodName}",
        messageFormat: $"Prefer {SelectManyMethodName} over {MatchMethodName}",
        category: nameof(Funcky),
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: string.Empty);

    /// <summary>Tests for a <c>Match</c> invocation of the shape <c>Match(none: Option&lt;T&gt;>.None, some: A)</c>.</summary>
    private static bool IsSelectManyEquivalent(AlternativeMonadType alternativeMonadType, IInvocationOperation matchInvocation, INamedTypeSymbol receiverType, IArgumentOperation noneArgument)
        => SymbolEqualityComparer.IncludeNullability.Equals(receiverType, matchInvocation.Type)
            && IsErrorStateConstructorReference(alternativeMonadType, noneArgument.Value);

    private static bool IsErrorStateConstructorReference(AlternativeMonadType alternativeMonadType, IOperation operation)
        => operation switch
        {
            IPropertyReferenceOperation propertyReference => IsErrorStateConstructorReference(alternativeMonadType, propertyReference),
            IDelegateCreationOperation { Target: IMethodReferenceOperation { Method: var method } } => IsErrorStateConstructorReference(alternativeMonadType, method),
            _ => false,
        };

    private static bool IsErrorStateConstructorReference(AlternativeMonadType alternativeMonadType, IMethodSymbol method)
        => method is { Name: var name, IsStatic: true, ContainingType: var containingType }
           && name == alternativeMonadType.ErrorStateConstructorName
           && IsTypeContainingConstructors(alternativeMonadType, containingType);

    private static bool IsErrorStateConstructorReference(AlternativeMonadType alternativeMonadType, IPropertyReferenceOperation operation)
        => operation is { Property: { Name: var name, IsStatic: true, ContainingType: var containingType } }
            && name == alternativeMonadType.ErrorStateConstructorName
            && IsTypeContainingConstructors(alternativeMonadType, containingType);

    private static bool IsTypeContainingConstructors(AlternativeMonadType alternativeMonadType, INamedTypeSymbol type)
        => SymbolEqualityComparer.Default.Equals(type.ConstructedFrom, alternativeMonadType.Type)
            || SymbolEqualityComparer.Default.Equals(type.ConstructedFrom, alternativeMonadType.ConstructorsType);
}
