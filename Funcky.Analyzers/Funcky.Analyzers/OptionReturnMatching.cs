using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace Funcky.Analyzers;

internal static class OptionReturnMatching
{
    public static bool IsOptionReturnFunction(IOperation operation)
        => operation switch
        {
            IDelegateCreationOperation delegateCreation => IsOptionReturnFunction(delegateCreation.Target),
            IAnonymousFunctionOperation anonymousFunction => IsOptionReturnFunction(anonymousFunction),
            IMethodReferenceOperation methodReference => IsOptionReturn(methodReference.Method, methodReference.SemanticModel),
            _ => false,
        };

    private static bool IsOptionReturn(IMethodSymbol method, SemanticModel? semanticModel)
        => method is { Name: "Return" or "Some", IsStatic: true, ContainingType: var methodType }
            && SymbolEqualityComparer.Default.Equals(methodType, semanticModel?.Compilation.GetOptionType());

    private static bool IsOptionReturnFunction(IAnonymousFunctionOperation anonymousFunction)
        => anonymousFunction.Body.Operations.Length == 1
            && anonymousFunction.Body.Operations[0] is IReturnOperation returnOperation
            && anonymousFunction.Symbol.Parameters.Length == 1
            && returnOperation.ReturnedValue is IInvocationOperation returnedValue
            && IsOptionReturn(returnedValue.TargetMethod, returnedValue.SemanticModel)
            && returnedValue.Arguments.Length == 1
            && returnedValue.Arguments[0].Value is IParameterReferenceOperation;
}
