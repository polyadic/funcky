using Funcky.Analyzers.CodeAnalysisExtensions;
using Microsoft.CodeAnalysis.Operations;
using static Funcky.Analyzers.FunckyWellKnownMemberNames;

namespace Funcky.Analyzers.FunctionalAssert;

internal sealed class XunitAssertMatching
{
    public static Option<(IArgumentOperation Expected, IArgumentOperation Actual)> MatchGenericAssertEqualInvocation(
        IInvocationOperation invocation)
        => invocation.TargetMethod.Name == XunitAssert.EqualMethodName
            && invocation.SemanticModel?.Compilation.GetXunitAssertType() is { } assertType
            && SymbolEquals(invocation.TargetMethod.ContainingType, assertType)
            && invocation.TargetMethod.TypeParameters is [var typeParameter]
            && invocation.GetArgumentsInParameterOrder() is [var expectedArgument, var actualArgument]
            && SymbolEquals(expectedArgument.Parameter?.OriginalDefinition.Type, typeParameter)
            && SymbolEquals(actualArgument.Parameter?.OriginalDefinition.Type, typeParameter)
                ? [(expectedArgument, actualArgument)] : [];
}
