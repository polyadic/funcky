using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace Funcky.Analyzers.Fallback;

internal static class OperationDescent
{
    /// <summary>The operation and its descendants, without descending into lambdas and local functions:
    /// what happens inside those is deferred and therefore not evaluated eagerly.</summary>
    public static IEnumerable<IOperation> DescendantsAndSelfOutsideFunctions(this IOperation operation)
    {
        yield return operation;

        foreach (var child in ChildOperations(operation))
        {
            if (child is not IAnonymousFunctionOperation and not ILocalFunctionOperation)
            {
                foreach (var descendant in child.DescendantsAndSelfOutsideFunctions())
                {
                    yield return descendant;
                }
            }
        }
    }

    private static IEnumerable<IOperation> ChildOperations(IOperation operation)
        => operation.ChildOperations;
}
