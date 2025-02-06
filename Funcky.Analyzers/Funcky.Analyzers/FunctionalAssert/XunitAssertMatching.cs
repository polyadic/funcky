using System.Diagnostics.CodeAnalysis;
using Funcky.Analyzers.CodeAnalysisExtensions;
using Microsoft.CodeAnalysis.Operations;
using static Funcky.Analyzers.FunckyWellKnownMemberNames;

namespace Funcky.Analyzers.FunctionalAssert;

internal sealed class XunitAssertMatching
{
    public static bool MatchGenericAssertEqualInvocation(
        IInvocationOperation invocation,
        [NotNullWhen(true)] out IArgumentOperation? expectedArgument,
        [NotNullWhen(true)] out IArgumentOperation? actualArgument)
    {
        expectedArgument = null;
        actualArgument = null;
        return invocation.TargetMethod.Name == XunitAssert.EqualMethodName
           && invocation.SemanticModel?.Compilation.GetXunitAssertType() is { } assertType
           && SymbolEquals(invocation.TargetMethod.ContainingType, assertType)
           && invocation.TargetMethod.TypeParameters is [var typeParameter]
           && invocation.GetArgumentsInParameterOrder() is [var expectedArgument_, var actualArgument_]
           && SymbolEquals(expectedArgument_.Parameter?.OriginalDefinition.Type, typeParameter)
           && SymbolEquals(actualArgument_.Parameter?.OriginalDefinition.Type, typeParameter)
           && (expectedArgument = expectedArgument_) is var _
           && (actualArgument = actualArgument_) is var _;
    }
}
