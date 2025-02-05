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

    public static bool IsIdentityFunctionWithNullConversion(IOperation operation)
        => operation is IDelegateCreationOperation { Target: IAnonymousFunctionOperation anonymousFunction }
           && MatchAnonymousUnaryFunctionWithSingleReturn(anonymousFunction, out var returnOperation)
           && returnOperation.ReturnedValue is IConversionOperation { Conversion.IsNullable: true, Operand: IParameterReferenceOperation { Parameter.ContainingSymbol: var parameterContainingSymbol } }
           && SymbolEquals(parameterContainingSymbol, anonymousFunction.Symbol);

    private static bool IsAnonymousIdentityFunction(IAnonymousFunctionOperation anonymousFunction)
        => MatchAnonymousUnaryFunctionWithSingleReturn(anonymousFunction, out var returnOperation)
            && returnOperation.ReturnedValue is IParameterReferenceOperation { Parameter.ContainingSymbol: var parameterContainingSymbol }
            && SymbolEquals(parameterContainingSymbol, anonymousFunction.Symbol);

    private static bool IsFunckyIdentityFunction(IMethodReferenceOperation methodReference)
        => methodReference.Method.Name == IdentityMethodName
            && SymbolEquals(
                methodReference.Method.ContainingType,
                methodReference.SemanticModel?.Compilation.GetFunctionalType());
}
