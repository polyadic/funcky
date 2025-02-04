using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
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
        const int expectedParameterIndex = 0;
        const int actualParameterIndex = 1;
        expectedArgument = null;
        actualArgument = null;
        return invocation.TargetMethod.Name == XunitAssert.EqualMethodName
           && invocation.SemanticModel?.Compilation.GetXunitAssertType() is { } assertType
           && SymbolEqualityComparer.Default.Equals(invocation.TargetMethod.ContainingType, assertType)
           && invocation.TargetMethod.TypeParameters is [var typeParameter]
           && invocation.TargetMethod.OriginalDefinition.Parameters is [var firstParameter, var secondParameter]
           && SymbolEqualityComparer.Default.Equals(firstParameter.Type, typeParameter)
           && SymbolEqualityComparer.Default.Equals(secondParameter.Type, typeParameter)
           && (expectedArgument = invocation.GetArgumentForParameterAtIndex(expectedParameterIndex)) is var _
           && (actualArgument = invocation.GetArgumentForParameterAtIndex(actualParameterIndex)) is var _;
    }
}
