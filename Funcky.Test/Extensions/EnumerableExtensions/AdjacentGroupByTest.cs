#pragma warning disable SA1010 // StyleCop support for collection expressions is missing
using Funcky.Test.TestUtilities;

namespace Funcky.Test.Extensions.EnumerableExtensions;

public sealed class AdjacentGroupByTest
{
    private const int DaysInAYear = 365;
    private const int DaysInALeapYear = 366;
    private const int MonthsInAYear = 12;
    private const int February = 1;
    private const int DaysInFebruaryInLeapYears = 29;

    [Fact]
    public void AdjacentGroupByIsEnumeratedLazily()
    {
        var doNotEnumerate = new FailOnEnumerationSequence<object>();

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
        var group = range.AdjacentGroupBy(_ => groupKey);

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

        Assert.Equal(DaysInALeapYear, dates.Count());
        Assert.Equal(MonthsInAYear, months.Count());
        Assert.Equal(DaysInMonthsOfALeapYear(), months.Select(month => month.Count()));
    }

    [Fact]
    public void GivenTwoYearsGroupByAdjacentGroupsJanuaryOfTwoDifferentYearsInTwoDifferentGroups()
    {
        var dates = DateGenerator(2019, 2020);

        var months = dates.AdjacentGroupBy(date => date.Month);

        Assert.Equal(DaysInAYear + DaysInALeapYear, dates.Count());
        Assert.Equal(2 * MonthsInAYear, months.Count());
        Assert.Equal(Sequence.Concat(DaysInMonthsOfAYear(), DaysInMonthsOfALeapYear()), months.Select(month => month.Count()));
    }

    [Fact]
    public void GivenAdjacentGroupByWithResultSelectorProjectsTheResultCorrectly()
    {
        var dates = DateGenerator(2020);

        var months = dates.AdjacentGroupBy(date => date.Month, (key, list) => list.Count());

        Assert.Equal(DaysInMonthsOfALeapYear(), months);
    }

    [Fact]
    public void GivenAdjacentGroupByWithElementSelectorProjectsTheResultCorrectly()
    {
        var numbers = Enumerable.Range(1, 5);

        var grouped = numbers.AdjacentGroupBy(number => number / 3, number => number * -1);

        Assert.Equal("-3,-4,-5", string.Join(",", grouped.Last()));
    }

    private static IEnumerable<DateTime> DateGenerator(int startYear, Option<int> endYear = default)
    {
        var current = new DateTime(startYear, 1, 1);

        while (current.Year <= endYear.GetOrElse(startYear))
        {
            yield return current;
            current = current.AddDays(1);
        }
    }

    private static IEnumerable<int> DaysInMonthsOfAYear()
        => [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];

    private static IEnumerable<int> DaysInMonthsOfALeapYear()
        => DaysInMonthsOfAYear()
            .Select((value, index) => index == February ? DaysInFebruaryInLeapYears : value);
}
