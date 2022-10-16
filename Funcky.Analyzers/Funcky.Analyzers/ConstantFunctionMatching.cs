using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using static Funcky.Analyzers.AnonymousFunctionMatching;

namespace Funcky.Analyzers;

internal static class ConstantFunctionMatching
{
    public static bool IsConstantFunction(IOperation operation, object? expectedValue)
        => operation switch
        {
            IDelegateCreationOperation delegateCreation => IsConstantFunction(delegateCreation.Target, expectedValue),
            IAnonymousFunctionOperation anonymousFunction => IsConstantFunction(anonymousFunction, expectedValue),
            _ => false,
        };

    private static bool IsConstantFunction(IAnonymousFunctionOperation anonymousFunction, object? expectedValue)
        => MatchAnonymousFunctionWithSingleReturn(anonymousFunction, out var returnOperation)
            && returnOperation.ReturnedValue is { ConstantValue: { HasValue: true, Value: var returnedValue } }
            && returnedValue == expectedValue;
}
