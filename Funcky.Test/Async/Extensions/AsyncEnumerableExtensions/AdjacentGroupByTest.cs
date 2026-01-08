using Funcky.Test.Async.TestUtilities;

namespace Funcky.Test.Async.Extensions.AsyncEnumerableExtensions;

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
        var doNotEnumerate = new FailOnEnumerateAsyncSequence<object>();

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
    public async Task GivenAnEmptySequenceAnyKeySelectorReturnsAnEmptySequence()
    {
        var empty = AsyncEnumerable.Empty<DateTime>();

        await AsyncAssert.Empty(empty.AdjacentGroupBy(date => date.Month));
        await AsyncAssert.Empty(empty.AdjacentGroupBy(date => date.DayOfYear / 7));
        await AsyncAssert.Empty(empty.AdjacentGroupBy(date => date.Year));
    }

    [Fact]
    public async Task GivenConstantKeySelectorOneGroupWithTheConstantsKeyWillBeSelected()
    {
        const int groupKey = 42;
        const int elementCount = 20;
        var range = AsyncEnumerable.Range(0, elementCount);
        var group = range.AdjacentGroupBy(_ => groupKey);

        var grouping = await AsyncAssert.Single(group);
        Assert.Equal(groupKey, grouping.Key);
        Assert.Equal(elementCount, await grouping.CountAsync());
    }

    [Fact]
    public async Task GivenASelectorSwitchingBetween0And1WeGetANewSequenceOnEachSwitch()
    {
        var range = AsyncEnumerable.Range(0, 100);

        Assert.Equal(25, await range.AdjacentGroupBy(n => (n / 4) % 2).CountAsync());
    }

    [Fact]
    public async Task GivenAYearGroupByCreatesMonthsCorrectly()
    {
        var dates = DateGenerator(2020);

        var months = dates.AdjacentGroupBy(date => date.Month);

        Assert.Equal(DaysInALeapYear, await dates.CountAsync());
        Assert.Equal(MonthsInAYear, await months.CountAsync());
        await AsyncAssert.Equal(DaysInMonthsOfALeapYear(), months.SelectAwait(async month => await month.CountAsync()));
    }

    [Fact]
    public async Task GivenTwoYearsGroupByAdjacentGroupsJanuaryOfTwoDifferentYearsInTwoDifferentGroups()
    {
        var dates = DateGenerator(2019, 2020);

        var months = dates.AdjacentGroupBy(date => date.Month);

        Assert.Equal(DaysInAYear + DaysInALeapYear, await dates.CountAsync());
        Assert.Equal(2 * MonthsInAYear, await months.CountAsync());
        await AsyncAssert.Equal(DaysInMonthsOfAYear().Concat(DaysInMonthsOfALeapYear()), months.SelectAwait(async month => await month.CountAsync()));
    }

    [Fact]
    public async Task GivenAdjacentGroupByWithResultSelectorProjectsTheResultCorrectly()
    {
        var dates = DateGenerator(2020);

        var months = dates.AdjacentGroupBy(date => date.Month, (_, list) => list.Count());

        await AsyncAssert.Equal(DaysInMonthsOfALeapYear(), months);
    }

    [Fact]
    public async Task GivenAdjacentGroupByWithElementSelectorProjectsTheResultCorrectly()
    {
        var numbers = AsyncEnumerable.Range(1, 5);

        var grouped = numbers.AdjacentGroupBy(number => number / 3, number => number * -1);
        Assert.Equal("-3,-4,-5", string.Join(",", await (await grouped.LastAsync()).ToListAsync()));
    }

#pragma warning disable CS1998
    private static async IAsyncEnumerable<DateTime> DateGenerator(int startYear, Option<int> endYear = default)
#pragma warning restore CS1998
    {
        var current = new DateTime(startYear, 1, 1);

        while (current.Year <= endYear.GetOrElse(startYear))
        {
            yield return current;
            current = current.AddDays(1);
        }
    }

    private static IAsyncEnumerable<int> DaysInMonthsOfAYear()
        => AsyncSequence.Return(31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31);

    private static IAsyncEnumerable<int> DaysInMonthsOfALeapYear()
        => DaysInMonthsOfAYear()
            .Select((value, index) => index == February ? DaysInFebruaryInLeapYears : value);
}
