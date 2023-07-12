using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using static Funcky.Analyzers.FunckyWellKnownMemberNames;
using static Funcky.Analyzers.IdentityFunctionMatching;

namespace Funcky.Analyzers;

public partial class AlternativeMonadAnalyzer
{
    public static readonly DiagnosticDescriptor PreferGetOrElse = new DiagnosticDescriptor(
        id: $"{DiagnosticName.Prefix}{DiagnosticName.Usage}05",
        title: $"Prefer {GetOrElseMethodName} over {MatchMethodName}",
        messageFormat: $"Prefer {GetOrElseMethodName} over {MatchMethodName}",
        category: nameof(Funcky),
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: string.Empty);

    /// <summary>Tests for a <c>Match</c> invocation of the shape <c>Match(none: A, some: Identity)</c>.</summary>
    private static bool IsGetOrElseEquivalent(INamedTypeSymbol receiverType, IArgumentOperation noneArgument, IArgumentOperation someArgument)
        => SymbolEqualityComparer.IncludeNullability.Equals(receiverType.TypeArguments.Last(), GetTypeOrDelegateReturnType(noneArgument.Value))
            && SymbolEqualityComparer.Default.Equals(receiverType.TypeArguments.Last(), GetTypeOrDelegateReturnType(someArgument.Value))
            && IsIdentityFunction(someArgument.Value);

    private static ITypeSymbol? GetTypeOrDelegateReturnType(IOperation operation)
        => operation switch
        {
            IDelegateCreationOperation { Target: IAnonymousFunctionOperation { Body.Operations: { Length: 1 } operations } } when operations[0] is IReturnOperation returnOperation => returnOperation.ReturnedValue?.Type,
            IDelegateCreationOperation { Target: IAnonymousFunctionOperation { Symbol.ReturnType: var returnType } } => returnType,
            IDelegateCreationOperation { Target: IMethodReferenceOperation { Method.ReturnType: var returnType } } => returnType,
            _ => operation.Type,
        };
}
