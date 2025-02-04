using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using static Funcky.Analyzers.FunckyWellKnownMemberNames;

namespace Funcky.Analyzers.FunctionalAssert;

internal sealed class XunitAssertMatching
{
    public static bool MatchGenericAssertEqualInvocation(IInvocationOperation invocation, [NotNullWhen(true)] out IArgumentOperation? actualArgument)
    {
        const int actualArgumentIndex = 1;
        actualArgument = null;
        return invocation.TargetMethod.Name == XunitAssert.EqualMethodName
           && invocation.SemanticModel?.Compilation.GetXunitAssertType() is { } assertType
           && SymbolEqualityComparer.Default.Equals(invocation.TargetMethod.ContainingType, assertType)
           && invocation.TargetMethod.TypeParameters is [var typeParameter]
           && invocation.TargetMethod.OriginalDefinition.Parameters is [var firstParameter, var secondParameter]
           && SymbolEqualityComparer.Default.Equals(firstParameter.Type, typeParameter)
           && SymbolEqualityComparer.Default.Equals(secondParameter.Type, typeParameter)
           && (actualArgument = invocation.GetArgumentForParameterAtIndex(actualArgumentIndex)) is var _;
    }
}
