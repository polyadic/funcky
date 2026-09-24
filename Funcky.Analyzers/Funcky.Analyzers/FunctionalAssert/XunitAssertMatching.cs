using Funcky.Analyzers.CodeAnalysisExtensions;
using Microsoft.CodeAnalysis.Operations;
using static Funcky.Analyzers.FunckyWellKnownMemberNames;
using static Funcky.Analyzers.OperationMatching;

namespace Funcky.Analyzers.FunctionalAssert;

internal sealed class XunitAssertMatching
{
    public static Option<(IArgumentOperation Expected, IArgumentOperation Actual)> MatchGenericAssertEqualInvocation(
        IInvocationOperation invocation)
        => MatchMethod(invocation, invocation.SemanticModel?.Compilation.GetXunitAssertType(), XunitAssert.EqualMethodName)
            && invocation.TargetMethod.TypeParameters is [var typeParameter]
            && invocation.GetArgumentsInParameterOrder() is [var expectedArgument, var actualArgument]
            && SymbolEquals(expectedArgument.Parameter?.OriginalDefinition.Type, typeParameter)
            && SymbolEquals(actualArgument.Parameter?.OriginalDefinition.Type, typeParameter)
                ? [(expectedArgument, actualArgument)] : [];
}
