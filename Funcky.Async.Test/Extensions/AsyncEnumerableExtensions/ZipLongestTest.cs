using Funcky.Async.Test.TestUtilities;

namespace Funcky.Async.Test.Extensions.AsyncEnumerableExtensions;

public sealed class ZipLongestTest
{
    [Fact]
    public void ZipLongestIsEnumeratedLazily()
    {
        var doNotEnumerate = new FailOnEnumerateAsyncSequence<object>();

        _ = doNotEnumerate.ZipLongest(doNotEnumerate, _ => Unit.Value);
    }

    [Fact]
    public async Task GivenTwoEmptySequencesZipLongestReturnsAnEmptySequence()
    {
        var numbers = AsyncEnumerable.Empty<int>();
        var strings = AsyncEnumerable.Empty<string>();

        var zipped = numbers.ZipLongest(strings, _ => Unit.Value);

        await AsyncAssert.Empty(zipped);
    }

    [Fact]
    public async Task GivenTwoSequencesOfTheSameLengthWeGetNoNoneValue()
    {
        var numbers = AsyncEnumerable.Range(0, 3);
        var strings = new[] { "Alpha", "Beta", "Gamma" }.ToAsyncEnumerable();

        var zipped = numbers
            .ZipLongest(strings);

        Assert.Equal(3, await zipped.CountAsync());
        await foreach (var value in zipped)
        {
            Assert.True(value.Match(left: False, right: False, both: True));
        }
    }

    [Fact]
    public async Task GivenTwoSequencesWeOfDifferentLengthWeGetTheLongerAndFillWithNone()
    {
        var numbers = AsyncEnumerable.Range(0, 10);
        var strings = new[] { "Alpha", "Beta", "Gamma" }.ToAsyncEnumerable();

        var zipped = numbers
            .ZipLongest(strings);

        Assert.Equal(10, await zipped.CountAsync());

        Assert.True((await zipped.FirstAsync()).Match(
            left: False,
            right: False,
            both: (left, right) => left == 0 && right == "Alpha"));

        Assert.True((await zipped.LastAsync()).Match(
            left: left => left == 9,
            right: False,
            both: False));
    }
}
