using Funcky.Analyzers.CodeAnalysisExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace Funcky.Analyzers.FunctionalAssert;

public sealed class FunctionalAssertMatching
{
    public static bool IsAssertMethodWithAccompanyingEqualOverload(
        IInvocationOperation invocation,
        AssertMethodHasOverloadWithExpectedValueAttributeType attributeType)
        => invocation.TargetMethod.HasAttribute(attributeType.Value) && invocation.Arguments.Length == 1;
}

public sealed record AssertMethodHasOverloadWithExpectedValueAttributeType(INamedTypeSymbol Value);
