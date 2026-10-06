using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using static Funcky.Analyzers.AlternativeMonad.AlternativeMonadErrorStateConstructorMatching;
using static Funcky.Analyzers.FunckyWellKnownMemberNames;
using static Funcky.Analyzers.Functions.AnonymousFunctionMatching;

namespace Funcky.Analyzers.AlternativeMonad;

public partial class AlternativeMonadAnalyzer
{
    public static readonly DiagnosticDescriptor PreferSelect = new(
        id: $"{DiagnosticName.Prefix}{DiagnosticName.Usage}11",
        title: $"Prefer {SelectMethodName} over {MatchMethodName}",
        messageFormat: $"Prefer {SelectMethodName} over {MatchMethodName}",
        category: nameof(Funcky),
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: string.Empty);

    /// <summary>Tests for a <c>Match</c> invocation of the shape <c>Match(none: Option&lt;TResult&gt;.None, some: x => A)</c>
    /// where <c>A</c> is implicitly converted to <c>Option&lt;TResult&gt;</c>.</summary>
    private static bool IsSelectEquivalent(AlternativeMonadType alternativeMonadType, IInvocationOperation matchInvocation, IArgumentOperation errorStateArgument, IArgumentOperation successStateArgument)
        => SymbolEquals(matchInvocation.Type?.OriginalDefinition, alternativeMonadType.Type)
            && IsErrorStateConstructorReference(alternativeMonadType, errorStateArgument.Value)
            && IsFunctionWithImplicitConversionToMonad(alternativeMonadType, successStateArgument.Value);

    private static bool IsFunctionWithImplicitConversionToMonad(AlternativeMonadType alternativeMonadType, IOperation operation)
        => operation is IDelegateCreationOperation { Target: IAnonymousFunctionOperation anonymousFunction }
            && MatchAnonymousUnaryFunctionWithSingleReturn(anonymousFunction) is [var returnOperation]
            && returnOperation.ReturnedValue is IConversionOperation { IsImplicit: true, Conversion.IsUserDefined: true, Type: var conversionType }
            && SymbolEquals(conversionType?.OriginalDefinition, alternativeMonadType.Type);
}
