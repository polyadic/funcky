using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace Funcky.Analyzers.FunctionalAssert;

public sealed class FunctionalAssertMatching
{
    public static bool IsAssertMethodWithAccompanyingEqualOverload(
        IInvocationOperation invocation,
        AssertMethodHasOverloadWithExpectedValueAttributeType attributeType)
        => invocation.TargetMethod.GetAttributes().Any(IsAttribute(attributeType.Value))
            && invocation.Arguments.Length == 1;

    private static Func<AttributeData, bool> IsAttribute(INamedTypeSymbol attributeType)
        => data => SymbolEquals(data.AttributeClass, attributeType);
}

public sealed record AssertMethodHasOverloadWithExpectedValueAttributeType(INamedTypeSymbol Value);
