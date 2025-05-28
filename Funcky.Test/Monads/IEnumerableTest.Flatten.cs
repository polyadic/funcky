namespace Funcky.Test.Monads;

public sealed partial class IEnumerableTest
{
    [Fact]
    public void FlattenFlatsIEnumerable()
    {
        var elements = new List<int> { 1, 2, 3 };
        Assert.Equal(elements, new List<IEnumerable<int>> { elements }.Flatten());
    }
}
