using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using static Funcky.Analyzers.AnonymousFunctionMatching;
using static Funcky.Analyzers.FunckyWellKnownMemberNames;

namespace Funcky.Analyzers;

internal static class IdentityFunctionMatching
{
    public static bool IsIdentityFunction(IOperation operation)
        => operation switch
        {
            IDelegateCreationOperation delegateCreation => IsIdentityFunction(delegateCreation.Target),
            IAnonymousFunctionOperation anonymousFunction => IsAnonymousIdentityFunction(anonymousFunction),
            IMethodReferenceOperation methodReference => IsFunckyIdentityFunction(methodReference),
            _ => false,
        };

    private static bool IsAnonymousIdentityFunction(IAnonymousFunctionOperation anonymousFunction)
        => MatchAnonymousUnaryFunctionWithSingleReturn(anonymousFunction, out var returnOperation)
            && returnOperation.ReturnedValue is IParameterReferenceOperation;

    private static bool IsFunckyIdentityFunction(IMethodReferenceOperation methodReference)
        => methodReference.Method.Name == IdentityMethodName
            && SymbolEqualityComparer.Default.Equals(
                methodReference.Method.ContainingType,
                methodReference.SemanticModel?.Compilation.GetFunctionalType());
}
