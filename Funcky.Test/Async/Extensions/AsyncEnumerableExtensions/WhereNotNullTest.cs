using Funcky.Test.Async.TestUtilities;

namespace Funcky.Test.Async.Extensions.AsyncEnumerableExtensions;

public sealed class WhereNotNullTest
{
    [Fact]
    public void WhereNotNullIsEnumeratedLazily()
    {
        var doNotEnumerate = new FailOnEnumerateAsyncSequence<object>();

        _ = doNotEnumerate.WhereNotNull();
    }

    [Fact]
    public Task WhereNotNullRemovesNullReferenceValues()
    {
        var input = AsyncSequence.Return(null, "foo", null, "bar", null);
        var expectedResult = AsyncSequence.Return("foo", "bar");

        return AsyncAssert.Equal(expectedResult, input.WhereNotNull());
    }

    [Fact]
    public Task WhereNotNullRemovesNullValueTypeValues()
    {
        var input = AsyncSequence.Return<int?>(null, 10, null, 20, null);
        var expectedResult = AsyncSequence.Return(10, 20);

        return AsyncAssert.Equal(expectedResult, input.WhereNotNull());
    }
}
