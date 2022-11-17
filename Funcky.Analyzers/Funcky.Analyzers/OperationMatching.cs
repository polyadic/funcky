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
        return operation.Arguments.InDeclarationOrder() is /*🎨*/[var first, var second]
            && matchFirstArgument(first)
            && matchSecondArgument(second)
            && (firstArgument = first) is var _
            && (secondArgument = second) is var _;
    }

    public static bool MatchField(
        IFieldReferenceOperation operation,
        INamedTypeSymbol type,
        string fieldName)
        => SymbolEqualityComparer.Default.Equals(operation.Type, type)
           && operation.Field.Name == fieldName;

    public static bool AnyArgument(IArgumentOperation operation) => true;

    public static Func<IArgumentOperation, bool> ConstantArgument(object? expectedValue)
        => argument
            => argument is { Value.ConstantValue: { HasValue: true, Value: var value } }
                && Equals(value, expectedValue);
}
