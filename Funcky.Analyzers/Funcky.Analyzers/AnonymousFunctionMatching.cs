using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace Funcky.Analyzers;

internal static class AnonymousFunctionMatching
{
    /// <summary>Matches an anonymous function of the shape <c>(x) => y</c>.</summary>
    public static bool MatchAnonymousUnaryFunctionWithSingleReturn(
        IAnonymousFunctionOperation anonymousFunction,
        [NotNullWhen(true)] out IReturnOperation? returnOperation)
    {
        returnOperation = null;
        return anonymousFunction.Body.Operations.Length == 1
            && anonymousFunction.Body.Operations[0] is IReturnOperation returnOperation_
            && anonymousFunction.Symbol.Parameters.Length == 1
            && (returnOperation = returnOperation_) is var _;
    }
}
