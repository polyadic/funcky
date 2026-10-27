namespace Funcky.Test.Extensions;

public sealed class TypeExtensionsTest
{
    [Fact]
    public void BaseTypeOrNoneReturnsTheBaseTypeOfAClass()
    {
        FunctionalAssert.Some(typeof(SystemException), typeof(InvalidOperationException).BaseTypeOrNone);
    }

    [Theory]
    [InlineData(typeof(object))]
    [InlineData(typeof(IDisposable))]
    public void BaseTypeOrNoneReturnsNoneForTypesWithoutABaseType(Type type)
    {
        FunctionalAssert.None(type.BaseTypeOrNone);
    }
}
