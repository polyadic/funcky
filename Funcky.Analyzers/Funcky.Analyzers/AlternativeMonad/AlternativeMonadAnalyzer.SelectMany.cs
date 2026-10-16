using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using static Funcky.Analyzers.AlternativeMonad.AlternativeMonadErrorStateConstructorMatching;
using static Funcky.Analyzers.FunckyWellKnownMemberNames;

namespace Funcky.Analyzers.AlternativeMonad;

public partial class AlternativeMonadAnalyzer
{
    public const string PreferSelectManyId = $"{DiagnosticName.Prefix}{DiagnosticName.Usage}07";

    public static readonly DiagnosticDescriptor PreferSelectMany = new(
        id: PreferSelectManyId,
        title: $"Prefer {SelectManyMethodName} over {MatchMethodName}",
        messageFormat: $"Prefer {SelectManyMethodName} over {MatchMethodName}",
        category: nameof(Funcky),
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: string.Empty,
        helpLinkUri: DiagnosticName.HelpLink(PreferSelectManyId));

    /// <summary>Tests for a <c>Match</c> invocation of the shape <c>Match(none: Option&lt;T&gt;>.None, some: A)</c>.</summary>
    private static bool IsSelectManyEquivalent(AlternativeMonadType alternativeMonadType, IInvocationOperation matchInvocation, INamedTypeSymbol receiverType, IArgumentOperation errorStateArgument)
        => SymbolEqualsIncludeNullability(receiverType, matchInvocation.Type)
            && IsErrorStateConstructorReference(alternativeMonadType, errorStateArgument.Value);
}
