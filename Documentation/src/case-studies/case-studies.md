# Case Studies

The reference chapters explain one function at a time. A case study does the opposite: it takes a small but
complete task and solves it end to end, so you can see how the pieces of Funcky fit together, which one to reach
for at which point, and what the resulting code looks like next to the conventional version.

Each study is self-contained and comes with the full program, which compiles against the current release.

| Case study                                              | Task                                                      | Features                                                                              |
|---------------------------------------------------------|-----------------------------------------------------------|---------------------------------------------------------------------------------------|
| [Simplify `if null` by using an `Option`](./if-null-to-option.md) | Refactor a chain of null checks step by step    | `Option`, `FromNullable`, `Select`, `AndThen`, `GetOrElse`                             |
| [Formatting a calendar page](./calendar.md)             | Lay out a yearly calendar as a streaming pipeline         | `Sequence.Successors`, `AdjacentGroupBy`, `Chunk`, `Transpose`, `JoinToString`, `Curry` |
| [Importing a CSV file](./csv-import.md)                 | Parse and validate records, report or reject bad lines    | `ParseOrNone`, `Either`, LINQ over monads, `Partition`, `Sequence`, `WithIndex`, `MaxByOrNone`, `Pairwise`, `InclusiveScan` |

If you have a task that you solved with Funcky and that shows an aspect none of these cover, a new case study is a
welcome contribution.
