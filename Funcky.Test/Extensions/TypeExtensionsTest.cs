namespace Funcky.Test.Extensions;

public sealed class TypeExtensionsTest
{
    [Fact]
    public void BaseTypeOrNoneReturnsTheBaseTypeOfAClass()
    {
        Assert.Some(typeof(SystemException), typeof(InvalidOperationException).BaseTypeOrNone);
    }

    [Theory]
    [InlineData(typeof(object))]
    [InlineData(typeof(IDisposable))]
    public void BaseTypeOrNoneReturnsNoneForTypesWithoutABaseType(Type type)
    {
        Assert.None(type.BaseTypeOrNone);
    }
}
