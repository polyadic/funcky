#pragma warning disable SA1010 // StyleCop support for collection expressions is missing
using Funcky.Test.TestUtilities;

namespace Funcky.Test.Extensions.EnumerableExtensions;

public sealed class WhereNotNullTest
{
    [Fact]
    public void WhereNotNullIsEnumeratedLazily()
    {
        var doNotEnumerate = new FailOnEnumerationSequence<object>();

        _ = doNotEnumerate.WhereNotNull();
    }

    [Fact]
    public void WhereNotNullRemovesNullReferenceValues()
    {
        IEnumerable<string?> input = [null, "foo", null, "bar", null];
        IEnumerable<string> expectedResult = ["foo", "bar"];

        Assert.Equal(expectedResult, input.WhereNotNull());
    }

    [Fact]
    public void WhereNotNullRemovesNullValueTypeValues()
    {
        IEnumerable<int?> input = [null, 10, null, 20, null];
        IEnumerable<int> expectedResult = [10, 20];

        Assert.Equal(expectedResult, input.WhereNotNull());
    }
}
