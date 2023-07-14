using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using static Funcky.Analyzers.FunckyWellKnownMemberNames;
using static Funcky.Analyzers.MonadReturnMatching;

namespace Funcky.Analyzers;

public partial class AlternativeMonadAnalyzer
{
    public static readonly DiagnosticDescriptor PreferOrElse = new DiagnosticDescriptor(
        id: $"{DiagnosticName.Prefix}{DiagnosticName.Usage}06",
        title: $"Prefer {OrElseMethodName} over {MatchMethodName}",
        messageFormat: $"Prefer {OrElseMethodName} over {MatchMethodName}",
        category: nameof(Funcky),
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: string.Empty);

    /// <summary>Tests for a <c>Match</c> invocation of the shape <c>Match(none: A, some: Option.Return)</c>.</summary>
    private static bool IsOrElseEquivalent(AlternativeMonadType alternativeMonadType, IInvocationOperation matchInvocation, INamedTypeSymbol receiverType, IArgumentOperation successStateArgument)
        => SymbolEqualityComparer.IncludeNullability.Equals(receiverType, matchInvocation.Type)
            && IsReturnFunction(alternativeMonadType, successStateArgument.Value);
}
