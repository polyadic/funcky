#if TRY_GET_NON_ENUMERATED_COUNT
using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using Funcky.Test.TestUtilities;

namespace Funcky.Test.Extensions.EnumerableExtensions;

public class GetNonEnumeratedCountOrNoneTest
{
    [Property]
    public Property GetNonEnumeratedCountOrNoneReturnsCountOnList(List<int> list)
    {
        return list.GetNonEnumeratedCountOrNone()
            .Match(none: false, some: count => count == list.Count)
            .ToProperty();
    }

    [Fact]
    public void GetNonEnumeratedCountOrNoneReturnsCountOnEnumerableRange()
    {
        var count = 20;

        var range = Enumerable.Range(1, count);

        FunctionalAssert.Some(count, range.GetNonEnumeratedCountOrNone());
    }

    [Property]
    public Property GetNonEnumeratedCountOrNoneReturnsNoneForInstancesWithoutCount(List<int> list)
    {
        return list.PreventLinqOptimizations().GetNonEnumeratedCountOrNone()
            .Match(none: true, some: False)
            .ToProperty();
    }
}
#endif
