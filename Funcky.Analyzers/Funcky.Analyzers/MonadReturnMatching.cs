using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using static Funcky.Analyzers.AnonymousFunctionMatching;
using static Funcky.Analyzers.FunckyWellKnownMemberNames;

namespace Funcky.Analyzers;

internal static class MonadReturnMatching
{
    public static bool IsReturnFunction(AlternativeMonadType alternativeMonadType, IOperation operation)
        => operation switch
        {
            IDelegateCreationOperation delegateCreation => IsReturnFunction(alternativeMonadType, delegateCreation.Target),
            IAnonymousFunctionOperation anonymousFunction => IsReturnFunction(alternativeMonadType, anonymousFunction) || IsImplicitReturn(alternativeMonadType, anonymousFunction),
            IMethodReferenceOperation methodReference => IsReturn(alternativeMonadType, methodReference.Method),
            _ => false,
        };

    private static bool IsReturn(AlternativeMonadType alternativeMonadType, IMethodSymbol method)
        => method is { Name: var name, IsStatic: true, ContainingType: var methodType }
            && (name is MonadReturnMethodName || name == alternativeMonadType.ReturnAlias)
            && (SymbolEqualityComparer.Default.Equals(methodType.ConstructedFrom, alternativeMonadType.Type)
                || SymbolEqualityComparer.Default.Equals(methodType.ConstructedFrom, alternativeMonadType.ConstructorsType));

    private static bool IsReturnFunction(AlternativeMonadType alternativeMonadType, IAnonymousFunctionOperation anonymousFunction)
        => MatchAnonymousUnaryFunctionWithSingleReturn(anonymousFunction, out var returnOperation)
           && returnOperation is { ReturnedValue: IInvocationOperation { Arguments: [{ Value: IParameterReferenceOperation { Parameter.ContainingSymbol: var parameterContainingSymbol } }] } returnedValue }
           && IsReturn(alternativeMonadType, returnedValue.TargetMethod)
           && SymbolEqualityComparer.Default.Equals(parameterContainingSymbol, anonymousFunction.Symbol);

    private static bool IsImplicitReturn(AlternativeMonadType alternativeMonadType, IAnonymousFunctionOperation anonymousFunction)
        => MatchAnonymousUnaryFunctionWithSingleReturn(anonymousFunction, out var returnOperation)
            && returnOperation is { ReturnedValue: IConversionOperation { IsImplicit: true, Operand: IParameterReferenceOperation, Type: var conversionType } }
            && SymbolEqualityComparer.Default.Equals(conversionType?.OriginalDefinition, alternativeMonadType.Type);
}
