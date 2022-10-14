using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using static Funcky.Analyzers.AnonymousFunctionMatching;
using static Funcky.Analyzers.FunckyWellKnownMemberNames;

namespace Funcky.Analyzers;

internal static class OptionReturnMatching
{
    public static bool IsOptionReturnFunction(IOperation operation)
        => operation switch
        {
            IDelegateCreationOperation delegateCreation => IsOptionReturnFunction(delegateCreation.Target),
            IAnonymousFunctionOperation anonymousFunction => IsOptionReturnFunction(anonymousFunction) || IsImplicitOptionReturn(anonymousFunction),
            IMethodReferenceOperation methodReference => IsOptionReturn(methodReference.Method, methodReference.SemanticModel),
            _ => false,
        };

    private static bool IsOptionReturn(IMethodSymbol method, SemanticModel? semanticModel)
        => method is { Name: MonadReturnMethodName or OptionSomeMethodName, IsStatic: true, ContainingType: var methodType }
            && SymbolEqualityComparer.Default.Equals(methodType, semanticModel?.Compilation.GetOptionType());

    private static bool IsOptionReturnFunction(IAnonymousFunctionOperation anonymousFunction)
        => MatchAnonymousUnaryFunctionWithSingleReturn(anonymousFunction, out var returnOperation)
            && returnOperation is { ReturnedValue: IInvocationOperation returnedValue }
            && IsOptionReturn(returnedValue.TargetMethod, returnedValue.SemanticModel)
            && returnedValue.Arguments.Length == 1
            && returnedValue.Arguments[0].Value is IParameterReferenceOperation;

    private static bool IsImplicitOptionReturn(IAnonymousFunctionOperation anonymousFunction)
        => MatchAnonymousUnaryFunctionWithSingleReturn(anonymousFunction, out var returnOperation)
            && returnOperation is { ReturnedValue: IConversionOperation { IsImplicit: true, Operand: IParameterReferenceOperation, Type: var conversionType } }
            && SymbolEqualityComparer.Default.Equals(conversionType?.OriginalDefinition, anonymousFunction.SemanticModel?.Compilation.GetOptionOfTType());
}
