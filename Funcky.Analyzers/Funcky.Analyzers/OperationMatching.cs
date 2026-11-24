using Funcky.Analyzers.ExtensionForms;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace Funcky.Analyzers;

internal static class OperationMatching
{
    /// <summary>Whether the invocation calls the method of that name declared in the class, directly or in one of its extension blocks.</summary>
    public static bool MatchMethod(
        IInvocationOperation operation,
        INamedTypeSymbol? type,
        string methodName)
        => SymbolEquals(ExtensionMemberSupport.DeclaringClass(operation.TargetMethod), type) && operation.TargetMethod.Name == methodName;

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
