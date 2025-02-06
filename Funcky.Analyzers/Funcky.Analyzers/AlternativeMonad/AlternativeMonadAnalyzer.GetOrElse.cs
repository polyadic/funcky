using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using static Funcky.Analyzers.FunckyWellKnownMemberNames;
using static Funcky.Analyzers.IdentityFunctionMatching;

namespace Funcky.Analyzers.AlternativeMonad;

public partial class AlternativeMonadAnalyzer
{
    public static readonly DiagnosticDescriptor PreferGetOrElse = new(
        id: $"{DiagnosticName.Prefix}{DiagnosticName.Usage}05",
        title: $"Prefer {GetOrElseMethodName} over {MatchMethodName}",
        messageFormat: $"Prefer {GetOrElseMethodName} over {MatchMethodName}",
        category: nameof(Funcky),
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: string.Empty);

    /// <summary>Tests for a <c>Match</c> invocation of the shape <c>Match(none: A, some: Identity)</c>.</summary>
    private static bool IsGetOrElseEquivalent(INamedTypeSymbol receiverType, IArgumentOperation errorStateArgument, IArgumentOperation successStateArgument)
        => SymbolEqualsIncludeNullability(receiverType.TypeArguments.Last(), GetTypeOrDelegateReturnType(errorStateArgument.Value))
            && SymbolEquals(receiverType.TypeArguments.Last(), GetTypeOrDelegateReturnType(successStateArgument.Value))
            && IsIdentityFunction(successStateArgument.Value);

    private static ITypeSymbol? GetTypeOrDelegateReturnType(IOperation operation)
        => operation switch
        {
            IDelegateCreationOperation { Target: IAnonymousFunctionOperation { Body.Operations: [IReturnOperation returnOperation] } } => returnOperation.ReturnedValue?.Type,
            IDelegateCreationOperation { Target: IAnonymousFunctionOperation { Symbol.ReturnType: var returnType } } => returnType,
            IDelegateCreationOperation { Target: IMethodReferenceOperation { Method.ReturnType: var returnType } } => returnType,
            _ => operation.Type,
        };
}
