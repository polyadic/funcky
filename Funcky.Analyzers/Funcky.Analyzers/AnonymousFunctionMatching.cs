using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace Funcky.Analyzers;

internal static class AnonymousFunctionMatching
{
    /// <summary>Matches an anonymous function of the shape <c>(x) => y</c>.</summary>
    public static bool MatchAnonymousUnaryFunctionWithSingleReturn(
        IAnonymousFunctionOperation anonymousFunction,
        [NotNullWhen(true)] out IReturnOperation? returnOperation)
        => MatchAnonymousFunctionWithSingleReturn(anonymousFunction, out returnOperation)
            && anonymousFunction.Symbol.Parameters.Length == 1;

    /// <summary>Matches an anonymous function of the shape <c>(...) => y</c>.</summary>
    public static bool MatchAnonymousFunctionWithSingleReturn(
        IAnonymousFunctionOperation anonymousFunction,
        [NotNullWhen(true)] out IReturnOperation? functionReturnOperation)
    {
        functionReturnOperation = null;
        return anonymousFunction.Body.Operations is /*🎨*/[IReturnOperation returnOperation]
            && (functionReturnOperation = returnOperation) is var _;
    }
}
