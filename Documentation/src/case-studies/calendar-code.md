
## Program.cs

The complete calendar program:

```cs
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Funcky.Extensions;
using System.Globalization;
using Funcky;
using static Funcky.Functional;

namespace Calendar
{
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
        private const int HorizontalMonths = 3;
        private const int DaysInAWeek = 7;

        private static void Main(string[] args)
            => Console.WriteLine(CreateCalendarString(2020));

        private static string CreateCalendarString(int year)
            => Sequence.Successors(JanuaryFirst(year), NextDay)
                .TakeWhile(SameYear(year))
                .AdjacentGroupBy(day => day.Month)
                .Select(LayoutMonth)
                .Chunk(HorizontalMonths)
                .Select(chunk => chunk.Transpose())
                .Select(JoinLine)
                .SelectMany(Identity)
                .JoinToString(Environment.NewLine);

        private static DateOnly NextDay(DateOnly day)
            => day.AddDays(1);

        private static DateOnly JanuaryFirst(int year)
            => new(year, 1, 1);

        private static Func<DateOnly, bool> SameYear(int year)
            => day
                => day.Year == year;

        private static IEnumerable<string> JoinLine(IEnumerable<IEnumerable<string>> sequence)
            => sequence.Select(t => t.JoinToString(" "));

        private static IEnumerable<string> LayoutMonth(IEnumerable<DateOnly> month)
            => ImmutableList<string>.Empty
                .Add(CenteredMonthName(month))
                .AddRange(FormatWeeks(month))
                .Add($"{string.Empty,21}");

        private static IEnumerable<string> FormatWeeks(IEnumerable<DateOnly> month)
            => month
                .AdjacentGroupBy(GetWeekOfYear)
                .Select(FormatWeek);

        private static string FormatWeek(IGrouping<int, DateOnly> week)
            => PadWeek(week.Select(FormatDay).ConcatToString(), week);

        private static string FormatDay(DateOnly day)
            => $"{day.Day,3}";

        private static string PadWeek(string formattedWeek, IGrouping<int, DateOnly> week)
            => StartsOnFirstDayOfWeek(week)
                ? $"{formattedWeek,21}"
                : $"{formattedWeek,-21}";

        private static bool StartsOnFirstDayOfWeek(IGrouping<int, DateOnly> week)
            => NthDayOfWeek(week.First().DayOfWeek) == 0;

        private static int NthDayOfWeek(DayOfWeek dayOfWeek)
            => (dayOfWeek + DaysInAWeek - CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek) % DaysInAWeek;

        private static int GetWeekOfYear(DateOnly dateTime)
            => CultureInfo
                .CurrentCulture
                .Calendar
                .GetWeekOfYear(dateTime.ToDateTime(default), CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule, CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek);

        private static string CenteredMonthName(IEnumerable<DateOnly> month)
            => month
                .First()
                .ToString("MMMM")
                .Center(21);
    }
}
```