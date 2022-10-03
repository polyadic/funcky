using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

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
        => anonymousFunction.Body.Operations.Length == 1
           && anonymousFunction.Body.Operations[0] is IReturnOperation returnOperation
           && anonymousFunction.Symbol.Parameters.Length == 1
           && returnOperation.ReturnedValue is IParameterReferenceOperation;

    private static bool IsFunckyIdentityFunction(IMethodReferenceOperation methodReference)
        => methodReference.Method.Name == "Identity"
            && SymbolEqualityComparer.Default.Equals(
                methodReference.Method.ContainingType,
                methodReference.SemanticModel?.Compilation.GetFunctionalType());
}
