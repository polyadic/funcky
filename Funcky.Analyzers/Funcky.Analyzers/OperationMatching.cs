using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace Funcky.Analyzers;

internal static class OperationMatching
{
    public static bool MatchMethod(
        IInvocationOperation operation,
        INamedTypeSymbol type,
        string methodName)
        => SymbolEqualityComparer.Default.Equals(operation.TargetMethod.ContainingType, type)
            && operation.TargetMethod.Name == methodName;

    public static bool MatchArguments(
        IInvocationOperation operation,
        [NotNullWhen(true)] out IArgumentOperation? firstArgument,
        Func<IArgumentOperation, bool> matchFirstArgument,
        [NotNullWhen(true)] out IArgumentOperation? secondArgument,
        Func<IArgumentOperation, bool> matchSecondArgument)
    {
        firstArgument = null;
        secondArgument = null;
        return operation.Arguments.Length is 2
            && matchFirstArgument(operation.Arguments[0])
            && matchSecondArgument(operation.Arguments[1])
            && (firstArgument = operation.Arguments[0]) is var _
            && (secondArgument = operation.Arguments[0]) is var _;
    }

    public static bool AnyArgument(IArgumentOperation operation) => true;

    public static Func<IArgumentOperation, bool> ConstantArgument(object? expectedValue)
        => argument
            => argument is { Value.ConstantValue: { HasValue: true, Value: var value } }
                && Equals(value, expectedValue);
}
