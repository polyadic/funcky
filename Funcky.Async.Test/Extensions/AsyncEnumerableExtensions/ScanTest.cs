using FsCheck;
using FsCheck.Xunit;
using Funcky.Async.Extensions;
using Funcky.Async.Test.TestUtilities;
using Xunit;

public class ScanTest
{
    [Fact]
    public void InclusiveScanIsEnumeratedLazily()
    {
        var doNotEnumerate = new FailOnEnumerateAsyncSequence<int>();

        _ = doNotEnumerate.InclusiveScan(0, AddElement);
    }

    [Property]
    public Property InclusiveScanOnAnEmptyListReturnsAnEmptyList(int neutral, Func<int, int, int> func)
    {
        var empty = AsyncEnumerable.Empty<int>();

        return empty.InclusiveScan(neutral, func).NoneAsync().Result.ToProperty();
    }

    [Property]
    public Property InclusiveScanCalculatesInclusivePrefixSum(int neutralElement, List<int> numbers)
        => InclusiveScanCheck(neutralElement, numbers.ToAsyncEnumerable())
            .Result
            .ToProperty();

    [Fact]
    public void ExclusiveScanIsEnumeratedLazily()
    {
        var doNotEnumerate = new FailOnEnumerateAsyncSequence<int>();

        _ = doNotEnumerate.ExclusiveScan(0, AddElement);
    }

    [Property]
    public Property ExclusiveScanOnAnEmptyListReturnsAnEmptyList(int neutral, Func<int, int, int> func)
    {
        var empty = AsyncEnumerable.Empty<int>();

        return empty.ExclusiveScan(neutral, func).NoneAsync().Result.ToProperty();
    }

    [Property]
    public Property ExclusiveScanCalculatesInclusivePrefixSum(int neutralElement, List<int> numbers)
        => ExclusiveScanCheck(neutralElement, numbers.ToAsyncEnumerable())
            .Result
            .ToProperty();

    private static async Task<bool> InclusiveScanCheck(int neutralElement, IAsyncEnumerable<int> numbers)
    {
        var result = true;
        var prefixSum = neutralElement;

        await foreach (var (element, inclusiveSum) in numbers.Zip(numbers.InclusiveScan(neutralElement, AddElement)))
        {
            prefixSum = AddElement(prefixSum, element);
            result = result && inclusiveSum == prefixSum;
        }

        return result;
    }

    private static async Task<bool> ExclusiveScanCheck(int neutralElement, IAsyncEnumerable<int> numbers)
    {
        var result = true;
        var prefixSum = neutralElement;

        await foreach (var (element, inclusiveSum) in numbers.Zip(numbers.InclusiveScan(neutralElement, AddElement)))
        {
            prefixSum = AddElement(prefixSum, element);
            result = result && inclusiveSum == prefixSum;
        }

        return result;
    }

    private static int AddElement(int sum, int element)
        => sum + element;
}
