# Importing a CSV file

Reading records from a text file is one of the most common tasks there is, and one where things go wrong in
the data rather than in the code: a date that does not exist, a number with a typo, a line with a field
missing. This case study imports a small bank statement and shows how Funcky handles those failures as
values, so the two questions "what is wrong" and "what do we do about it" get answered in separate places.

The input has a header line, then one transaction per line with a date, a description and an amount. Three
of the lines are broken on purpose:

```
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
```

February 30th is not a date, `-2x40` is not an amount, and the bike repair has no amount at all.

We want two things from the import. First, every line is either a `Transaction` or a description of what is
wrong with it, with the line number, so the person who maintains the file can fix it. Second, the caller
decides the policy: skip the bad lines and import the rest, or reject the whole file as soon as one line is
broken. Both policies should use the same parser.

The complete program is listed on the [Program.cs](./csv-import-code.md) page.

## Reading the lines

`SplitLines` from the string extensions splits the text into lines lazily, whatever the line ending. We skip
the header, and because the error messages need line numbers, we attach the index of each remaining line with
`WithIndex`. The result is a sequence of `ValueWithIndex<string>`, a small struct with the `Value` and its
`Index`.

```cs
var lines = csv
    .SplitLines()
    .Skip(1)
    .WithIndex()
    .Select(ParseLine);
```

`ParseLine` turns one indexed line into an `Either<ImportError, Transaction>`. Nothing has happened yet when
this statement completes, the whole pipeline is deferred until something consumes it.

## Parsing one line

A line is valid when it has exactly three fields, the first one parses as a date and the third one as a
decimal. The description can be anything. The result type is `Either<ImportError, Transaction>`, where
`ImportError` is a plain record with the line number and a message:

```cs
record Transaction(DateOnly Date, string Description, decimal Amount);

record ImportError(int LineNumber, string Message);
```

Why `Either` rather than `Result`? `Result<T>` carries an `Exception` as its failure, which is the right
choice when the failure comes from an API that throws. Here the failure is a domain value we construct
ourselves, with no stack trace and no exception type to choose, so `Either<TLeft, TRight>` with our own left
type fits better. The convention is that the left side is the failure and the right side the value.

The parser itself is one expression:

```cs
static Either<ImportError, Transaction> ParseLine(ValueWithIndex<string> line)
    => line.Value.Split(',') is [var date, var description, var amount]
        ? from parsedDate in date.ParseDateOnlyOrNone(InvariantCulture).ToEither(Error(line, $"'{date}' is not a date"))
          from parsedAmount in amount.ParseDecimalOrNone(NumberStyles.Number, InvariantCulture).ToEither(Error(line, $"'{amount}' is not an amount"))
          select new Transaction(parsedDate, description, parsedAmount)
        : Either<ImportError, Transaction>.Left(Error(line, "expected 3 fields"));

// The header is line 1 and was skipped, so the first indexed line is line 2.
static ImportError Error(ValueWithIndex<string> line, string message)
    => new(line.Index + 2, message);
```

Three things are happening here.

The list pattern `[var date, var description, var amount]` checks the field count and names the fields in one
go. If it does not match, the line is a `Left` right away.

`ParseDateOnlyOrNone` and `ParseDecimalOrNone` are the [TryVerb-pattern](../try-pattern.md) wrappers from the
parse extensions. They return an `Option` instead of a `bool` plus an `out` parameter, so they can be used
inside an expression. An `Option` says "there is no date", but not why, so `ToEither` upgrades it to an
`Either` by supplying the error for the `None` case. The error carries the offending text, which is the single
most useful thing to put into a message of this kind.

The two `from` clauses are LINQ query syntax over `Either`. Every monad in Funcky implements `Select` and
`SelectMany`, which is all the compiler needs to allow `from … in … select`. The query reads like the happy
path: take the date, take the amount, build the transaction. If any `from` source is a `Left`, the remaining
clauses are skipped and that `Left` is the result of the whole expression. This is the same short-circuiting
the [Option chapter](../option.md) describes, now with an error attached.

Note that no step in `ParseLine` can throw. A broken line is a value like any other and the pipeline keeps
flowing.

## Two import policies

With the parser in place, the policy decision is a single call on the sequence of `Either` values.

The lenient policy keeps everything that parsed and reports the rest. `Partition` on an
`IEnumerable<Either<TLeft, TRight>>` evaluates the sequence once and splits it into the left values and the
right values, which deconstruct into two lists:

```cs
var (errors, transactions) = lines.Partition();
foreach (var error in errors)
{
    Console.WriteLine($"line {error.LineNumber}: {error.Message}");
}

Console.WriteLine(Report(transactions));
```

The strict policy rejects the file at the first broken line. `Sequence` turns an
`IEnumerable<Either<TLeft, TRight>>` inside out into an `Either<TLeft, IReadOnlyList<TRight>>`: either all
lines were right and you get the list, or you get the first left. It stops enumerating at that line, so a
broken line early in a large file costs nothing further. `Match` then produces the message or the report:

```cs
Console.WriteLine(lines.Sequence().Match(
    left: error => $"import rejected, line {error.LineNumber}: {error.Message}",
    right: Report));
```

Both policies consume the same lazily built `lines`. The parser does not know which policy will be applied,
and the policy does not know how a line is parsed. The two overloads exist for `Result<T>` as well, with
`Error` and `Ok` partitions, so a parser that wraps a throwing API fits the same two calls.

## The report

The imported transactions feed a small report. Each line of it is one Funcky operation on the list, and the
`Report` function only assembles them:

```cs
static string Report(IReadOnlyList<Transaction> transactions)
    => Sequence
        .Return(
            $"{transactions.Count} transactions imported",
            MonthlyTotals(transactions),
            LargestIncome(transactions),
            LongestGap(transactions),
            $"final balance: {Balance(transactions):F2}")
        .JoinToString(Environment.NewLine);
```

`Sequence.Return` builds a sequence from its arguments and `JoinToString` is the `string.Join` you can put at
the end of a pipeline.

Monthly totals use `AdjacentGroupBy`, the same forward-only grouping as in the
[calendar](./calendar.md#group-this-into-months). A statement is ordered by date, so transactions of the same
month are adjacent and there is no need to sort or to build a dictionary. The overload with a result selector
receives the key and the group's elements and projects each group directly into its output line:

```cs
static string MonthlyTotals(IEnumerable<Transaction> transactions)
    => transactions
        .AdjacentGroupBy(
            transaction => (transaction.Date.Year, transaction.Date.Month),
            (month, items) => $"  {month.Year}-{month.Month:00}: {items.Sum(transaction => transaction.Amount),9:F2}")
        .Prepend("monthly totals:")
        .JoinToString(Environment.NewLine);
```

The largest income is the element with the largest amount. `MaxByOrNone` returns `None` for an empty list
instead of throwing, and `Match` turns both cases into a line of text:

```cs
static string LargestIncome(IEnumerable<Transaction> transactions)
    => transactions
        .MaxByOrNone(transaction => transaction.Amount)
        .Match(
            none: "largest income: none",
            some: transaction => $"largest income: {transaction.Description} ({transaction.Amount:F2})");
```

The longest gap between two transactions needs each element together with its predecessor. `Pairwise` yields
exactly that, and with a result selector it computes the distance in days right away. `MaxOrNone` handles the
case of fewer than two transactions, where there is no pair at all:

```cs
static string LongestGap(IEnumerable<Transaction> transactions)
    => transactions
        .Pairwise((earlier, later) => later.Date.DayNumber - earlier.Date.DayNumber)
        .MaxOrNone()
        .Match(
            none: "longest gap: fewer than two transactions",
            some: days => $"longest gap between two transactions: {days} days");
```

The final balance is a running sum. `InclusiveScan` is `Aggregate` that yields every intermediate value, which
is what you would need to print a balance column. Here we only keep the last one, and `LastOrNone` with
`GetOrElse` makes an empty import a balance of zero rather than an exception:

```cs
static decimal Balance(IEnumerable<Transaction> transactions)
    => transactions
        .InclusiveScan(0m, (balance, transaction) => balance + transaction.Amount)
        .LastOrNone()
        .GetOrElse(0m);
```

## Output

Running the program prints:

```
== Lenient import: skip the bad lines ==
line 7: '2026-02-30' is not a date
line 8: '-2x40' is not an amount
line 9: expected 3 fields
7 transactions imported
monthly totals:
  2026-01:   2545.10
  2026-02:   -180.00
  2026-03:   4102.55
largest income: Salary (4200.00)
longest gap between two transactions: 29 days
final balance: 6467.65

== Strict import: all or nothing ==
import rejected, line 7: '2026-02-30' is not a date
```

## What this shows

* Parsing with `…OrNone` instead of `TryParse` keeps the parser an expression.
* `ToEither` is the bridge from "no value" to "no value, and here is why".
* LINQ query syntax works over `Either`, `Option` and `Result`, and short-circuits on the failure case.
* `Partition` and `Sequence` are the two policies for a sequence of fallible values: collect everything, or
  fail at the first problem. The parser does not change between them.
* `AdjacentGroupBy`, `Pairwise` and `InclusiveScan` express grouping, neighbour comparison and running totals
  without index arithmetic.
* Every `…OrNone` in the report makes the empty case explicit, so an empty file produces a report rather than
  an `InvalidOperationException` from `Max` or `Last`.
