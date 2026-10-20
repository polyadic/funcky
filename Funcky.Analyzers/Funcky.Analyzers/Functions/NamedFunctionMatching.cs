using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using static Funcky.Analyzers.Functions.AnonymousFunctionMatching;

namespace Funcky.Analyzers.Functions;

/// <summary>Recognizes anonymous functions that are spelled out versions of one of the named functions in <c>Funcky.Functional</c>.</summary>
internal static class NamedFunctionMatching
{
    public const string TrueMethodName = "True";

    public const string FalseMethodName = "False";

    public const string NoOperationMethodName = "NoOperation";

    public static NamedFunction? MatchNamedFunction(IAnonymousFunctionOperation anonymousFunction)
        => anonymousFunction.Symbol switch
        {
            { IsAsync: true } => null,
            { ReturnsVoid: true, Parameters.Length: var parameterCount } when HasEmptyBody(anonymousFunction)
                => new NamedFunction(NoOperationMethodName, parameterCount),
            { Parameters: [var parameter] } when ReturnsParameter(anonymousFunction, parameter)
                => new NamedFunction(FunckyWellKnownMemberNames.IdentityMethodName, ParameterCount: 1),
            { Parameters.Length: var parameterCount } when ReturnsBooleanLiteral(anonymousFunction) is { } value
                => new NamedFunction(value ? TrueMethodName : FalseMethodName, parameterCount),
            _ => null,
        };

    /// <summary>An empty block; the compiler may append an implicit return to it.</summary>
    private static bool HasEmptyBody(IAnonymousFunctionOperation anonymousFunction)
        => anonymousFunction.Body.Operations.All(operation => operation is IReturnOperation { IsImplicit: true, ReturnedValue: null });

    /// <summary>The returned value must be the parameter itself: a conversion (e.g. to a base type or to a nullable) is not the identity.</summary>
    private static bool ReturnsParameter(IAnonymousFunctionOperation anonymousFunction, IParameterSymbol parameter)
        => MatchAnonymousFunctionWithSingleReturn(anonymousFunction) is [{ ReturnedValue: IParameterReferenceOperation parameterReference }]
            && SymbolEquals(parameterReference.Parameter, parameter);

    /// <summary>The returned value must be a literal: a constant expression such as a <c>const</c> field is not a true/false function by intent.</summary>
    private static bool? ReturnsBooleanLiteral(IAnonymousFunctionOperation anonymousFunction)
        => MatchAnonymousFunctionWithSingleReturn(anonymousFunction) is [{ ReturnedValue: ILiteralOperation { ConstantValue: { HasValue: true, Value: bool value } } }]
            ? value
            : null;

    public sealed record NamedFunction(string Name, int ParameterCount);
}
