
## Program.cs

The complete calendar program:

```cs
using System.Collections.Immutable;
using Funcky.Extensions;
using Funcky;
using Funcky.Monads;
using static System.Globalization.CultureInfo;
using static Funcky.Functional;

namespace Calendar;

internal static class StringExtensions
{
    public static string Center(this string text, int width)
        => (width - text.Length) switch
        {
            0 => text,
            1 => $" {text}",
            _ => Center($" {text} ", width),
        };
}

internal class Program
{
    private const int FirstDayOfTheWeek = 0;
    private const int MonthsPerRow = 3;
    private const int DaysInAWeek = 7;
    private const int WidthOfDay = 3;
    private const int WidthOfAWeek = WidthOfDay * DaysInAWeek;
    private const string MonthNameFormat = "MMMM";

    private static void Main(string[] args)
        => Console.WriteLine(CreateCalendarString(ExtractYear(args).GetOrElse(DateTime.Now.Year)));

    private static Option<int> ExtractYear(IEnumerable<string> args)
        => args.FirstOrNone()
            .AndThen(ParseExtensions.ParseInt32OrNone);

    private static string CreateCalendarString(int year)
        => Sequence.Successors(JanuaryFirst(year), NextDay)
            .TakeWhile(IsSameYear(year))
            .AdjacentGroupBy(day => day.Month)
            .Select(LayoutMonth)
            .Chunk(MonthsPerRow)
            .Select(EnumerableExtensions.Transpose)
            .Select(JoinLine)
            .SelectMany(Identity)
            .JoinToString(Environment.NewLine);

    private static DateOnly NextDay(DateOnly day)
        => day.AddDays(1);

    private static DateOnly JanuaryFirst(int year)
        => new(year: year, month: 1, day: 1);

    private static Func<DateOnly, bool> IsSameYear(int year)
        => day
            => day.Year == year;

    private static IEnumerable<string> JoinLine(IEnumerable<IEnumerable<string>> sequence)
        => sequence.Select(t => t.JoinToString(" "));

    private static IEnumerable<string> LayoutMonth(IEnumerable<DateOnly> month)
        => ImmutableList<string>.Empty
            .Add(CenteredMonthName(month))
            .AddRange(FormatWeeks(month))
            .Add(new string(' ', WidthOfAWeek));

    private static IEnumerable<string> FormatWeeks(IEnumerable<DateOnly> month)
        => month
            .AdjacentGroupBy(GetWeekOfYear)
            .Select(FormatWeek);

    private static string FormatWeek(IGrouping<int, DateOnly> week)
        => PadWeek(week.Select(FormatDay).ConcatToString(), week);

    private static string FormatDay(DateOnly day)
        => $"{day.Day,WidthOfDay}";

    private static string PadWeek(string formattedWeek, IEnumerable<DateOnly> week)
        => StartsOnFirstDayOfWeek(week)
            ? $"{formattedWeek,-WidthOfAWeek}"
            : $"{formattedWeek,WidthOfAWeek}";

    private static bool StartsOnFirstDayOfWeek(IEnumerable<DateOnly> week)
        => NthDayOfWeek(week.First().DayOfWeek) is FirstDayOfTheWeek;

    private static int NthDayOfWeek(DayOfWeek dayOfWeek)
        => (dayOfWeek + DaysInAWeek - CurrentCulture.DateTimeFormat.FirstDayOfWeek) % DaysInAWeek;

    private static int GetWeekOfYear(DateOnly dateTime)
        => CurrentCulture
            .Calendar
            .GetWeekOfYear(
                dateTime.ToDateTime(default),
                CurrentCulture.DateTimeFormat.CalendarWeekRule,
                CurrentCulture.DateTimeFormat.FirstDayOfWeek);

    private static string CenteredMonthName(IEnumerable<DateOnly> month)
        => month
            .First()
            .ToString(MonthNameFormat)
            .Center(WidthOfAWeek);
}
```