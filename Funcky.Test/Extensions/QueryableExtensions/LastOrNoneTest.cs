using Funcky.Test.TestUtils;

namespace Funcky.Test.Extensions.QueryableExtensions;

public sealed class LastOrNoneTest
{
    [Fact]
    public void LastOrNoneIsEvaluatedUsingExpressions()
        => _ = Enumerable.Empty<int>()
            .AsQueryable()
            .PreventAccidentalUseAsEnumerable()
            .LastOrNone();
}
