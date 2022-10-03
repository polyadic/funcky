using Microsoft.CodeAnalysis.Operations;

namespace Funcky.Analyzers;

internal static class ArgumentOperationExtensions
{
    public static IArgumentOperation GetArgumentForParameterAtIndex(this IInvocationOperation invocationOperation, int parameterIndex)
        => GetArgumentForParameterAtIndex(invocationOperation.Arguments, parameterIndex);

    private static IArgumentOperation GetArgumentForParameterAtIndex(this IEnumerable<IArgumentOperation> arguments, int parameterIndex)
        => arguments.Single(argument => argument.Parameter?.Ordinal == parameterIndex);
}
