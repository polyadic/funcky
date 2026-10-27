namespace Funcky.Test.Extensions;

public sealed class MemberInfoExtensionsTest
{
    [Fact]
    public void DeclaringTypeOrNoneReturnsTheDeclaringTypeOfANestedTypeAndOfAMethod()
    {
        FunctionalAssert.Some(typeof(MemberInfoExtensionsTest), typeof(Nested).DeclaringTypeOrNone);
        FunctionalAssert.Some(typeof(string), typeof(string).GetMethod(nameof(string.Trim), Type.EmptyTypes)!.DeclaringTypeOrNone);
    }

    [Fact]
    public void DeclaringTypeOrNoneReturnsNoneForATypeThatIsNotNested()
    {
        FunctionalAssert.None(typeof(MemberInfoExtensionsTest).DeclaringTypeOrNone);
    }

    private sealed class Nested;
}
