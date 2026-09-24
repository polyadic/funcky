using Microsoft.CodeAnalysis.Operations;

namespace Funcky.Analyzers.Functions;

internal static class AnonymousFunctionMatching
{
    /// <summary>Matches an anonymous function of the shape <c>(x) => y</c>.</summary>
    public static Option<IReturnOperation> MatchAnonymousUnaryFunctionWithSingleReturn(
        IAnonymousFunctionOperation anonymousFunction)
        => MatchAnonymousFunctionWithSingleReturn(anonymousFunction) is [var returnOperation]
            && anonymousFunction.Symbol.Parameters is [_]
                ? [returnOperation] : [];

    /// <summary>Matches an anonymous function of the shape <c>(...) => y</c>.</summary>
    public static Option<IReturnOperation> MatchAnonymousFunctionWithSingleReturn(IAnonymousFunctionOperation anonymousFunction)
        => anonymousFunction.Body.Operations is [IReturnOperation returnOperation]
            ? [returnOperation] : [];
}
