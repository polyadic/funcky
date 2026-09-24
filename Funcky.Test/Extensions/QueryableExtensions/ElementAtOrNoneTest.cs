using FsCheck;
using FsCheck.Xunit;
#if ELEMENT_AT_INDEX
using Funcky.FsCheck;
#endif
using Funcky.Test.TestUtilities;

namespace Funcky.Test.Extensions.QueryableExtensions;

public sealed class ElementAtOrNoneTest
{
    public sealed class IntIndex
    {
        [Property]
        public void IsEvaluatedUsingExpressions(List<int> source, int index)
            => _ = source
                .AsQueryable()
                .PreventAccidentalUseAsEnumerable()
                .ElementAtOrNone(index);

        [Property]
        public Property BehavesTheSameAsElementAt(List<int> source, int index)
        {
            var queryable = source.AsQueryable();

            try
            {
                var expectedValue = queryable.ElementAt(index);
                return CheckAssert.Some(expectedValue, queryable.ElementAtOrNone(index));
            }
            catch (ArgumentOutOfRangeException)
            {
                return CheckAssert.None(queryable.ElementAtOrNone(index));
            }
        }
    }

#if ELEMENT_AT_INDEX
    public sealed class IndexIndex
    {
        [FunckyProperty]
        public void IsEvaluatedUsingExpressions(List<int> source, Index index)
            => _ = source
                .AsQueryable()
                .PreventAccidentalUseAsEnumerable()
                .ElementAtOrNone(index);

        [FunckyProperty]
        public Property BehavesTheSameAsElementAt(List<int> source, Index index)
        {
            var queryable = source.AsQueryable();

            try
            {
                var expectedValue = queryable.ElementAt(index);
                return CheckAssert.Some(expectedValue, queryable.ElementAtOrNone(index));
            }
            catch (ArgumentOutOfRangeException)
            {
                return CheckAssert.None(queryable.ElementAtOrNone(index));
            }
        }
    }
#endif
}
