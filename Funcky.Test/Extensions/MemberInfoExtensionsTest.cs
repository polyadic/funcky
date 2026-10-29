namespace Funcky.Test.Extensions;

public sealed class MemberInfoExtensionsTest
{
    [Fact]
    public void DeclaringTypeOrNoneReturnsTheDeclaringTypeOfANestedTypeAndOfAMethod()
    {
        Assert.Some(typeof(MemberInfoExtensionsTest), typeof(Nested).DeclaringTypeOrNone);
        Assert.Some(typeof(string), typeof(string).GetMethod(nameof(string.Trim), Type.EmptyTypes)!.DeclaringTypeOrNone);
    }

    [Fact]
    public void DeclaringTypeOrNoneReturnsNoneForATypeThatIsNotNested()
    {
        Assert.None(typeof(MemberInfoExtensionsTest).DeclaringTypeOrNone);
    }

    private sealed class Nested;
}
