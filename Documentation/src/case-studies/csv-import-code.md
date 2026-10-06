## Program.cs

The complete import program:

```cs
using System.Globalization;
using Funcky;
using Funcky.Extensions;
using Funcky.Monads;
using static System.Globalization.CultureInfo;

CurrentCulture = InvariantCulture;

const string csv = """
    Date,Description,Amount
    2026-01-03,Salary,4200.00
    2026-01-05,Groceries,-83.20
    2026-01-19,Rent,-1450.00
    2026-01-31,Groceries,-121.70
    2026-02-01,Dentist,-180
    2026-02-30,Refund,35.00
    2026-02-14,Concert tickets,-2x40
    2026-02-17,Bike repair
    2026-03-02,Salary,4200.00
    2026-03-06,Groceries,-97.45
    """;

var lines = csv
    .SplitLines()
    .Skip(1)
    .WithIndex()
    .Select(ParseLine);

Console.WriteLine("== Lenient import: skip the bad lines ==");
var (errors, transactions) = lines.Partition();
foreach (var error in errors)
{
    Console.WriteLine($"line {error.LineNumber}: {error.Message}");
}

Console.WriteLine(Report(transactions));

Console.WriteLine();
Console.WriteLine("== Strict import: all or nothing ==");
Console.WriteLine(lines.Sequence().Match(
    left: error => $"import rejected, line {error.LineNumber}: {error.Message}",
    right: Report));

static Either<ImportError, Transaction> ParseLine(ValueWithIndex<string> line)
    => line.Value.Split(',') is [var date, var description, var amount]
        ? from parsedDate in date.ParseDateOnlyOrNone(InvariantCulture).ToEither(Error(line, $"'{date}' is not a date"))
          from parsedAmount in amount.ParseDecimalOrNone(NumberStyles.Number, InvariantCulture).ToEither(Error(line, $"'{amount}' is not an amount"))
          select new Transaction(parsedDate, description, parsedAmount)
        : Either<ImportError, Transaction>.Left(Error(line, "expected 3 fields"));

// The header is line 1 and was skipped, so the first indexed line is line 2.
static ImportError Error(ValueWithIndex<string> line, string message)
    => new(line.Index + 2, message);

static string Report(IReadOnlyList<Transaction> transactions)
    => Sequence
        .Return(
            $"{transactions.Count} transactions imported",
            MonthlyTotals(transactions),
            LargestIncome(transactions),
            LongestGap(transactions),
            $"final balance: {Balance(transactions):F2}")
        .JoinToString(Environment.NewLine);

static string MonthlyTotals(IEnumerable<Transaction> transactions)
    => transactions
        .AdjacentGroupBy(
            transaction => (transaction.Date.Year, transaction.Date.Month),
            (month, items) => $"  {month.Year}-{month.Month:00}: {items.Sum(transaction => transaction.Amount),9:F2}")
        .Prepend("monthly totals:")
        .JoinToString(Environment.NewLine);

static string LargestIncome(IEnumerable<Transaction> transactions)
    => transactions
        .MaxByOrNone(transaction => transaction.Amount)
        .Match(
            none: "largest income: none",
            some: transaction => $"largest income: {transaction.Description} ({transaction.Amount:F2})");

static string LongestGap(IEnumerable<Transaction> transactions)
    => transactions
        .Pairwise((earlier, later) => later.Date.DayNumber - earlier.Date.DayNumber)
        .MaxOrNone()
        .Match(
            none: "longest gap: fewer than two transactions",
            some: days => $"longest gap between two transactions: {days} days");

static decimal Balance(IEnumerable<Transaction> transactions)
    => transactions
        .InclusiveScan(0m, (balance, transaction) => balance + transaction.Amount)
        .LastOrNone()
        .GetOrElse(0m);

record Transaction(DateOnly Date, string Description, decimal Amount);

record ImportError(int LineNumber, string Message);
```
