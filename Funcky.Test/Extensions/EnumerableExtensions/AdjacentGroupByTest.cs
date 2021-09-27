using Funcky.Test.TestUtils;

namespace Funcky.Test.Extensions.EnumerableExtensions
{
    public sealed class AdjacentGroupByTest
    {
        [Fact]
        public void AdjacentGroupByIsEnumeratedLazily()
        {
            var doNotEnumerate = new FailOnEnumerateSequence<object>();

            _ = doNotEnumerate.AdjacentGroupBy(FailOnCall.Function<object, int>);
            _ = doNotEnumerate.AdjacentGroupBy(FailOnCall.Function<object, int>, EqualityComparer<int>.Default);
            _ = doNotEnumerate.AdjacentGroupBy(FailOnCall.Function<object, int>, FailOnCall.Function<object, object>);
            _ = doNotEnumerate.AdjacentGroupBy(FailOnCall.Function<object, int>, FailOnCall.Function<int, IEnumerable<object>, IEnumerable<object>>);
            _ = doNotEnumerate.AdjacentGroupBy(FailOnCall.Function<object, int>, FailOnCall.Function<object, object>, EqualityComparer<int>.Default);
            _ = doNotEnumerate.AdjacentGroupBy(FailOnCall.Function<object, int>, FailOnCall.Function<int, IEnumerable<object>, IEnumerable<object>>, EqualityComparer<int>.Default);
            _ = doNotEnumerate.AdjacentGroupBy(FailOnCall.Function<object, int>, FailOnCall.Function<object, object>, FailOnCall.Function<int, IEnumerable<object>, IEnumerable<object>>);
            _ = doNotEnumerate.AdjacentGroupBy(FailOnCall.Function<object, int>, FailOnCall.Function<object, object>, FailOnCall.Function<int, IEnumerable<object>, IEnumerable<object>>, EqualityComparer<int>.Default);
        }

        [Fact]
        public void GivenAnEmptySequenceAnyKeySelectorReturnsAnEmptySequence()
        {
            var empty = Enumerable.Empty<DateTime>();

            Assert.Empty(empty.AdjacentGroupBy(date => date.Month));
            Assert.Empty(empty.AdjacentGroupBy(date => date.DayOfYear / 7));
            Assert.Empty(empty.AdjacentGroupBy(date => date.Year));
        }

        [Fact]
        public void GivenConstantKeySelectorOneGroupWithTheConstantsKeyWillBeSelected()
        {
            const int groupKey = 42;
            const int elementCount = 20;
            var range = Enumerable.Range(0, elementCount);
            var group = range.AdjacentGroupBy(n => groupKey);

            var grouping = Assert.Single(group);
            Assert.Equal(groupKey, grouping.Key);
            Assert.Equal(elementCount, grouping.Count());
        }

        [Fact]
        public void GivenASelectorSwitchingBetween0And1WeGetANewSequenceOnEachSwitch()
        {
            var range = Enumerable.Range(0, 100);

            Assert.Equal(25, range.AdjacentGroupBy(n => (n / 4) % 2).Count());
        }

        [Fact]
        public void GivenAYearGroupByCreatesMonthsCorrectly()
        {
            var dates = DateGenerator(2020);

            var months = dates.AdjacentGroupBy(date => date.Month);

            Assert.Equal(366, dates.Count());
            Assert.Equal(12, months.Count());
            Assert.Equal(new[] { 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 }, months.Select(month => month.Count()));
        }

        [Fact]
        public void GivenTwoYearsGroupByAdjacentGroupsJanuaryOfTwoDifferentYearsInTwoDifferentGroups()
        {
            var dates = DateGenerator(2019, 2020);

            var months = dates.AdjacentGroupBy(date => date.Month);

            Assert.Equal(731, dates.Count());
            Assert.Equal(24, months.Count());
            Assert.Equal(new[] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31, 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 }, months.Select(month => month.Count()));
        }

        [Fact]
        public void GivenAdjacentGroupByWithResultSelectorProjectsTheResultCorrectly()
        {
            var dates = DateGenerator(2020);

            var months = dates.AdjacentGroupBy(date => date.Month, (key, list) => list.Count());

            Assert.Equal(new[] { 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 }, months);
        }

        [Fact]
        public void GivenAdjacentGroupByWithElementSelectorProjectsTheResultCorrectly()
        {
            var numbers = Enumerable.Range(1, 5);

            var grouped = numbers.AdjacentGroupBy(number => number / 3, number => number * -1);

            Assert.Equal("-3,-4,-5", string.Join(",", grouped.Last()));
        }

        private IEnumerable<DateTime> DateGenerator(int startYear, Option<int> endYear = default)
        {
            var current = new DateTime(startYear, 1, 1);

            while (current.Year <= endYear.GetOrElse(startYear))
            {
                yield return current;
                current = current.AddDays(1);
            }
        }
    }
}
