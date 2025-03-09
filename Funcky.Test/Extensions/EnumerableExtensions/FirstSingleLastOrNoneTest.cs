#pragma warning disable SA1010 // StyleCop support for collection expressions is missing
using System.Collections;
using Funcky.Test.TestUtils;
using Xunit.Sdk;

namespace Funcky.Test.Extensions.EnumerableExtensions;

public sealed class FirstSingleLastOrNoneTest
{
    [Theory]
    [MemberData(nameof(ValueReferenceEnumerables))]
    public void GivenAnValueEnumerableFirstLastOrNoneGivesTheCorrectOption(List<int> valueEnumerable, List<string> referenceEnumerable)
    {
        Assert.Equal(ExpectedOptionValue(valueEnumerable), valueEnumerable.FirstOrNone().Match(none: false, some: True));
        Assert.Equal(ExpectedOptionValue(referenceEnumerable), referenceEnumerable.FirstOrNone().Match(none: false, some: True));

        Assert.Equal(ExpectedOptionValue(valueEnumerable), valueEnumerable.LastOrNone().Match(none: false, some: True));
        Assert.Equal(ExpectedOptionValue(referenceEnumerable), referenceEnumerable.LastOrNone().Match(none: false, some: True));
    }

    [Theory]
    [MemberData(nameof(ValueReferenceEnumerables))]
    public void GivenAnEnumerableSingleOrNoneGivesTheCorrectOption(List<int> valueEnumerable, List<string> referenceEnumerable)
    {
        ExpectedSingleOrNoneBehaviour(valueEnumerable, () => valueEnumerable.SingleOrNone().Match(none: false, some: True));
        ExpectedSingleOrNoneBehaviour(valueEnumerable, () => referenceEnumerable.SingleOrNone().Match(none: false, some: True));
    }

    [Fact]
    public void DoesNotEnumerateListsWhenCalledWithoutPredicate()
    {
        var list = new FailOnEnumerateListWrapper<string>(["foo"]);
        _ = list.FirstOrNone();
        _ = list.LastOrNone();

        // SingleOrNone does not specialize for `Select`ed lists.
        Assert.Throws<XunitException>(() => _ = list.SingleOrNone());
    }

    public static TheoryData<List<int>, List<string>> ValueReferenceEnumerables()
        => new()
        {
            { [], [] },
            { [1], ["a"] },
            { [1, 2, 3], ["a", "b", "c"] },
        };

    private static bool ExpectedOptionValue(ICollection valueEnumerable) =>
        valueEnumerable.Count switch
        {
            0 => false,
            _ => true,
        };

    private static void ExpectedSingleOrNoneBehaviour(ICollection list, Func<bool> singleOrNone)
    {
        switch (list.Count)
        {
            case 0:
                Assert.False(singleOrNone());
                break;
            case 1:
                Assert.True(singleOrNone());
                break;
            default:
                Assert.Throws<InvalidOperationException>(() => singleOrNone());
                break;
        }
    }
}
