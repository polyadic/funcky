using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using static Funcky.Analyzers.FunckyWellKnownMemberNames;
using static Funcky.Analyzers.OptionReturnMatching;

namespace Funcky.Analyzers;

public partial class OptionMatchAnalyzer
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
    private static bool IsOrElseEquivalent(IInvocationOperation matchInvocation, INamedTypeSymbol receiverType, IArgumentOperation someArgument)
        => SymbolEqualityComparer.IncludeNullability.Equals(receiverType, matchInvocation.Type)
            && IsOptionReturnFunction(someArgument.Value);
}
