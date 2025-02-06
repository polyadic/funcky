using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace Funcky.Analyzers;

internal static class OperationMatching
{
    public static bool MatchMethod(
        IInvocationOperation operation,
        INamedTypeSymbol type,
        string methodName)
        => SymbolEquals(operation.TargetMethod.ContainingType, type) && operation.TargetMethod.Name == methodName;

    public static bool MatchField(
        IFieldReferenceOperation operation,
        INamedTypeSymbol type,
        string fieldName)
        => SymbolEquals(operation.Type, type) && operation.Field.Name == fieldName;

    public static bool MatchConstantArgument(IArgumentOperation argument, object? expectedValue)
        => argument is { Value.ConstantValue: { HasValue: true, Value: var value } } && Equals(value, expectedValue);

    public static Func<IArgumentOperation, bool> ConstantArgument(object? expectedValue)
        => argument => MatchConstantArgument(argument, expectedValue);
}
