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
    private static bool IsSelectManyEquivalent(IInvocationOperation matchInvocation, INamedTypeSymbol receiverType, IArgumentOperation noneArgument)
        => SymbolEqualityComparer.IncludeNullability.Equals(receiverType, matchInvocation.Type)
            && IsOptionNoneExpression(noneArgument.Value);

    private static bool IsOptionNoneExpression(IOperation operation)
        => operation is IPropertyReferenceOperation { Property: { Name: OptionNonePropertyName, IsStatic: true, ContainingType: var type } }
            && SymbolEqualityComparer.Default.Equals(type.ConstructedFrom, operation.SemanticModel?.Compilation.GetOptionOfTType());
}
