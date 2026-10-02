# IEnumerable Extensions

LINQ gives you the essential higher-order functions over `IEnumerable<T>`: `Select`, `Where`, `SelectMany`,
`Aggregate`, and a few dozen more. Funcky adds the ones that are missing, and replaces the ones that return
`null`, `default` or throw with variants that return an `Option<T>`.

All of them live in `Funcky.Extensions.EnumerableExtensions`, which is imported automatically when
`FunckyImplicitUsings` is on. Most of them are lazy, in the same way LINQ's own operators are: nothing happens
until the result is enumerated, and the input is enumerated at most once. The exceptions are noted on each
operation's page.

Operations marked with ⁂ have an asynchronous counterpart on `IAsyncEnumerable<T>`, in `Funcky` itself on
.NET 10 and in the `Funcky.Async` package on older targets.

## Finding one element

The `…OrNone` family: like `…OrDefault`, but with an unambiguous answer for "nothing found".
See [Finding one element](./finding-one-element.md).

| Operation                                                                    | Description                                                      |
|------------------------------------------------------------------------------|------------------------------------------------------------------|
| [FirstOrNone](./finding-one-element.md#firstornone) ⁂                        | The first element, or the first matching a predicate             |
| [LastOrNone](./finding-one-element.md#lastornone) ⁂                          | The last element, or the last matching a predicate               |
| [SingleOrNone](./finding-one-element.md#singleornone) ⁂                      | The only element; throws if there is more than one               |
| [ElementAtOrNone](./finding-one-element.md#elementatornone) ⁂                | The element at an index, also from the end                       |
| [MinOrNone](./finding-one-element.md#minornone) ⁂                            | The smallest element or key                                      |
| [MaxOrNone](./finding-one-element.md#maxornone) ⁂                            | The largest element or key                                       |
| [MinByOrNone](./finding-one-element.md#minbyornone)                          | The element with the smallest key                                |
| [MaxByOrNone](./finding-one-element.md#maxbyornone)                          | The element with the largest key                                 |
| [AverageOrNone](./finding-one-element.md#averageornone) ⁂                    | The arithmetic mean                                              |
| [GetNonEnumeratedCountOrNone](./finding-one-element.md#getnonenumeratedcountornone) | The count, if it is known without enumerating             |

## Grouping and windowing

Operations that cut one sequence into several. See [Grouping and windowing](./grouping-and-windowing.md).

| Operation                                                                    | Description                                                      |
|------------------------------------------------------------------------------|------------------------------------------------------------------|
| [Chunk](./grouping-and-windowing.md#chunk) ⁂                                 | Consecutive pieces of a fixed size                               |
| [SlidingWindow](./grouping-and-windowing.md#slidingwindow) ⁂ | Overlapping windows of a fixed width                             |
| [Pairwise](./grouping-and-windowing.md#pairwise) ⁂                           | Each element together with its successor                         |
| [AdjacentGroupBy](./grouping-and-windowing.md#adjacentgroupby) ⁂             | Group runs of consecutive elements with the same key             |
| [Split](./grouping-and-windowing.md#split) ⁂ | Cut at every occurrence of a separator                           |
| [TakeEvery](./grouping-and-windowing.md#takeevery) ⁂ | Every n-th element                                               |
| [Transpose](./grouping-and-windowing.md#transpose) ⁂ | Swap rows and columns of a sequence of sequences                 |
| [PowerSet](./grouping-and-windowing.md#powerset) ⁂                           | All subsets                                                      |

## Combining sequences

Operations that take several sequences and produce one. See [Combining sequences](./combining-sequences.md).

| Operation                                                                    | Description                                                      |
|------------------------------------------------------------------------------|------------------------------------------------------------------|
| [Merge](./combining-sequences.md#merge) ⁂                                    | Merge sorted sequences, keeping them sorted                      |
| [Interleave](./combining-sequences.md#interleave) ⁂                          | Alternate elements from several sequences                        |
| [Intersperse](./combining-sequences.md#intersperse) ⁂                        | Put a separator element between every two elements               |
| [ZipLongest](./combining-sequences.md#ziplongest) ⁂ | Zip two sequences without truncating the longer one              |
| Flatten                                                                      | Concatenate a sequence of sequences                              |
| [CartesianProduct](./combining-sequences.md#cartesianproduct)                | All pairs, as a recipe with `SelectMany`                         |

## Context for each element

Operations that pair every element with information about its position. See [Context for each element](./element-context.md).

| Operation                                                                    | Description                                                      |
|------------------------------------------------------------------------------|------------------------------------------------------------------|
| [WithIndex](./element-context.md#withindex) ⁂                                | Each element with its zero-based index                           |
| [WithFirst](./element-context.md#withfirst) ⁂                                | Each element with a flag telling whether it is the first         |
| [WithLast](./element-context.md#withlast) ⁂                                  | Each element with a flag telling whether it is the last          |
| [WithPrevious](./element-context.md#withprevious) ⁂                          | Each element with its predecessor as an `Option`                 |
| InclusiveScan, ExclusiveScan ⁂ | Running aggregate, like `Aggregate` yielding every intermediate  |

## Filtering and partitioning

See [Filtering and partitioning](./filtering-and-partitioning.md).

| Operation                                                                    | Description                                                      |
|------------------------------------------------------------------------------|------------------------------------------------------------------|
| [WhereSelect](./filtering-and-partitioning.md#whereselect) ⁂                 | `Select` with an `Option`-returning selector, dropping the `None`s |
| [WhereNotNull](./filtering-and-partitioning.md#wherenotnull) ⁂               | Drop the `null`s and narrow the element type                     |
| [Partition](./filtering-and-partitioning.md#partition) ⁂                     | Split into two lists by a predicate, or by `Either`/`Result` case |
| [None](./filtering-and-partitioning.md#none) ⁂                               | `!Any`, as a readable name                                       |
| AnyOrElse ⁂ | The sequence itself, or a fallback if it is empty                |

## Side effects and evaluation

Operations that run actions, or control when and how often the input is enumerated.
See [Side effects and evaluation](./side-effects-and-evaluation.md).

| Operation                                                                    | Description                                                      |
|------------------------------------------------------------------------------|------------------------------------------------------------------|
| [ForEach](./side-effects-and-evaluation.md#foreach) | Run an action for each element, eagerly                          |
| [Inspect](./side-effects-and-evaluation.md#inspect) ⁂                        | Run an action for each element as it passes through, lazily      |
| InspectEmpty ⁂ | Run an action if the sequence turns out to be empty              |
| [Materialize](./side-effects-and-evaluation.md#materialize) ⁂                | Evaluate once into a collection, unless it already is one        |
| Memoize ⁂ | Evaluate lazily, but at most once, even for several consumers    |
| [Shuffle](./side-effects-and-evaluation.md#shuffle) ⁂                        | A random permutation                                             |

## Strings and monads

| Operation                                                                    | Description                                                      |
|------------------------------------------------------------------------------|------------------------------------------------------------------|
| JoinToString ⁂ | `string.Join` as an extension                                    |
| ConcatToString ⁂ | `string.Concat` as an extension                                  |
| Sequence                                                                     | Turn a sequence of `Option`/`Result`/`Either` inside out         |
| Traverse                                                                     | `Select` with a monadic selector, then `Sequence`                |
