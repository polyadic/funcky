using Funcky.Async.Extensions;
using Funcky.Async.Test.TestUtilities;

namespace Funcky.Async.Test.Extensions.AsyncEnumerableExtensions;

public sealed class WhereNotNullTest
{
    [Fact]
    public void WhereNotNullIsEnumeratedLazily()
    {
        var doNotEnumerate = new FailOnEnumerateAsyncSequence<object>();

        _ = doNotEnumerate.WhereNotNull();
    }

    [Fact]
    public async Task WhereNotNullRemovesNullReferenceValues()
    {
        var input = new[]
        {
            null,
            "foo",
            null,
            "bar",
            null,
        }.ToAsyncEnumerable();
        var expectedResult = new[]
        {
            "foo",
            "bar",
        }.ToAsyncEnumerable();

        await AsyncAssert.Equal(expectedResult, input.WhereNotNull());
    }

    [Fact]
    public async Task WhereNotNullRemovesNullValueTypeValues()
    {
        var input = new int?[]
        {
            null,
            10,
            null,
            20,
            null,
        }.ToAsyncEnumerable();
        var expectedResult = new[]
        {
            10,
            20,
        }.ToAsyncEnumerable();

        await AsyncAssert.Equal(expectedResult, input.WhereNotNull());
    }
}
