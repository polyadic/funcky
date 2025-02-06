using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Operations;

namespace Funcky.Analyzers.CodeAnalysisExtensions;

internal static class ArgumentOperationExtensions
{
    /// <summary>Gets the arguments in the order that the parameters were declared.
    /// Optimal for pattern matching using a list pattern.</summary>
    public static ArgumentsInParameterOrder GetArgumentsInParameterOrder(this IInvocationOperation invocationOperation)
        => new(invocationOperation);
}

#pragma warning disable SA1201
internal readonly struct ArgumentsInParameterOrder(IInvocationOperation invocationOperation)
#pragma warning restore SA1201
{
    private readonly Lazy<ImmutableArray<IArgumentOperation>> _sortedArguments
        = new(SortArguments(invocationOperation.Arguments));

    public int Length => invocationOperation.Arguments.Length;

    public IArgumentOperation this[int index] => _sortedArguments.Value[index];

    private static Func<ImmutableArray<IArgumentOperation>> SortArguments(ImmutableArray<IArgumentOperation> arguments)
        => () => arguments.Sort((x, y) => Comparer<int?>.Default.Compare(x.Parameter?.Ordinal, y.Parameter?.Ordinal));
}
