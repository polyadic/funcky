using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using Funcky.Test.TestUtils;

namespace Funcky.Test.Extensions.AsyncEnumerableExtensions;

public class ScanTest
{
    [Fact]
    public void InclusiveScanIsEnumeratedLazily()
    {
        var doNotEnumerate = new FailOnEnumerationSequence<int>();

        _ = doNotEnumerate.InclusiveScan(0, AddElement);
    }

    [Property]
    public Property InclusiveScanOnAnEmptyListReturnsAnEmptyList(int neutral, Func<int, int, int> func)
    {
        var empty = Enumerable.Empty<int>();

        return empty.InclusiveScan(neutral, func).None().ToProperty();
    }

    [Property]
    public Property InclusiveScanCalculatesInclusivePrefixSum(int neutralElement, List<int> numbers)
    {
        var result = true;
        var prefixSum = neutralElement;

        foreach (var (element, inclusiveSum) in numbers.Zip(numbers.InclusiveScan(neutralElement, AddElement), Tuple.Create))
        {
            prefixSum = AddElement(prefixSum, element);
            result = result && inclusiveSum == prefixSum;
        }

        return result.ToProperty();
    }

    [Fact]
    public void ExclusiveScanIsEnumeratedLazily()
    {
        var doNotEnumerate = new FailOnEnumerationSequence<int>();

        _ = doNotEnumerate.ExclusiveScan(0, AddElement);
    }

    [Property]
    public Property ExclusiveScanOnAnEmptyListReturnsAnEmptyList(int neutral, Func<int, int, int> func)
    {
        var empty = Enumerable.Empty<int>();

        return empty.ExclusiveScan(neutral, func).None().ToProperty();
    }

    [Property]
    public Property ExclusiveScanCalculatesExclusivePrefixSum(int neutralElement, List<int> numbers)
    {
        var result = true;
        var prefixSum = neutralElement;

        foreach (var (element, exclusiveSum) in numbers.Zip(numbers.ExclusiveScan(neutralElement, AddElement), Tuple.Create))
        {
            result = result && exclusiveSum == prefixSum;
            prefixSum = AddElement(prefixSum, element);
        }

        return result.ToProperty();
    }

    private static int AddElement(int sum, int element)
        => sum + element;
}
