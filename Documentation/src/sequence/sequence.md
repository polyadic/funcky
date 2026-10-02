# Sequence Constructors

LINQ is good at transforming sequences and poor at creating them. `Enumerable.Range` and `Enumerable.Repeat`
are the only constructors it offers, so code that needs a one-element sequence ends up with `new[] { x }`,
and code that needs an unfolding sequence ends up with a `while` loop and `yield`. The static class
`Funcky.Sequence` fills that gap.

All members are static methods on `Sequence`, so they are called as `Sequence.Return(...)` and so on. Those
that take a function or a lazy input are lazy themselves; `Return` and `FromNullable` are small materialized
lists.

| Constructor                                        | Produces                                                        |
|----------------------------------------------------|-----------------------------------------------------------------|
| [Return](#return)                                  | One element, or exactly the elements given                      |
| [FromNullable](#fromnullable)                      | One element, or nothing when it is `null`                       |
| [Successors](#successors)                          | Each element computed from the previous one; `Aggregate` in reverse |
| [Cycle](#cycle)                                    | The same element forever                                        |
| [CycleRange, CycleMaterialized](#cyclerange-and-cyclematerialized) | A whole sequence, repeated forever               |
| [RepeatRange, RepeatMaterialized](#repeatrange-and-repeatmaterialized) | A whole sequence, repeated *n* times         |
| [Concat](#concat)                                  | Several sequences one after the other                           |

## Return

Wraps one or more values in a sequence.

```cs
IReadOnlyList<TResult> Return<TResult>(TResult element)
IReadOnlyList<TResult> Return<TResult>(params TResult[] elements)
```

`Return` is the name monads use for "put a plain value into the monad", and `IEnumerable<T>` is a monad. Where
you would write `Option.Return(x)` or `Result.Return(x)`, you write `Sequence.Return(x)` for a sequence. It
replaces `new[] { x }`, `new List<T> { x }` and `Enumerable.Repeat(x, 1)`; the analyzer [λ1001](../analyzer-rules/λ1001.md)
flags the last one.

The `params` overload gives you a literal sequence of several elements, which is why the examples throughout
this book use it. The result is an `IReadOnlyList<T>`, so it is materialized and can be enumerated any number
of times.

```cs
IEnumerable<string> greeting = Sequence.Return("hello");

var suits = Sequence.Return("♠", "♣", "♥", "♦");

// Prepend a header to a lazy sequence without materializing it
var report = Sequence.Return(header).Concat(rows);
```

## FromNullable

A sequence of one element, or an empty sequence if the element is `null`.

```cs
IEnumerable<TResult> FromNullable<TResult>(TResult? element) where TResult : class
IEnumerable<TResult> FromNullable<TResult>(TResult? element) where TResult : struct
```

This is `Option.FromNullable` followed by `ToEnumerable`, in one step. It is useful where a `null` should
simply contribute nothing, for example inside a `SelectMany`:

```cs
var inner = exceptions.SelectMany(e => Sequence.FromNullable(e.InnerException));
```

## Successors

Builds a sequence by applying a function to the previous element, starting from a given first element. It is
the inverse of `Aggregate`: `Aggregate` folds a sequence into a value, `Successors` unfolds a value into a
sequence.

```cs
IEnumerable<TResult> Successors<TResult>(TResult first, Func<TResult, TResult> successor)
IEnumerable<TResult> Successors<TResult>(TResult first, Func<TResult, Option<TResult>> successor)
IEnumerable<TResult> Successors<TResult>(Option<TResult> first, Func<TResult, TResult> successor)
IEnumerable<TResult> Successors<TResult>(Option<TResult> first, Func<TResult, Option<TResult>> successor)
```

<picture>
    <picture>
      <source srcset="successors-dark.svg" media="(prefers-color-scheme: dark)">
      <img src="successors.svg" alt="A marble diagram showing the Successors operation">
    </picture>
</picture>

With a successor returning `T`, the sequence is infinite, and you must `Take`, `TakeWhile` or otherwise stop
it. With a successor returning `Option<T>`, the sequence ends at the first `None`; the element that produced
the `None` is still included. The overloads taking an `Option<T>` as the first element produce an empty
sequence when it is `None`, which is convenient when the start itself comes from an `…OrNone` lookup.

The first element is always included. `Skip(1)` if you only want what follows it.

```cs
// All non-negative integers
Sequence.Successors(0, n => n + 1);

// Powers of two that fit in an int
Sequence.Successors(1, n => n <= int.MaxValue / 2 ? Option.Some(n * 2) : Option<int>.None);

// The chain of inner exceptions
Sequence.Successors(exception, e => Option.FromNullable(e.InnerException));

// Walking up a directory tree
Sequence.Successors(directory, d => Option.FromNullable(d.Parent));

// Fibonacci, by carrying a pair of state and projecting it away
Sequence.Successors((0, 1), p => (p.Item2, p.Item1 + p.Item2)).Select(p => p.Item1);

// All days of a year, see the calendar case study
Sequence.Successors(JanuaryFirst(year), NextDay).TakeWhile(day => day.Year == year);
```

The last example is a pattern worth recognising: anything you would express with a `for` loop whose state is
one value, `for (var x = first; ...; x = next(x))`, is a `Successors`.

## Cycle

Repeats a single element forever.

```cs
IEnumerable<TResult> Cycle<TResult>(TResult element)
```

This is `Enumerable.Repeat` without a count, or `Successors(element, Identity)`. Typical uses are zipping
a constant against another sequence, or a placeholder stream to be cut with `Take`.

```cs
var padded = words.ZipLongest(Sequence.Cycle("").Take(10), pair => pair.Match(
    left: w => w, right: p => p, both: (w, _) => w));
```

## CycleRange and CycleMaterialized

Repeat a whole sequence forever: after the last element, the first comes again.

```cs
IBuffer<TSource> CycleRange<TSource>(IEnumerable<TSource> source)
IEnumerable<TSource> CycleMaterialized<TSource>(IReadOnlyCollection<TSource> source)
```

<picture>
    <picture>
      <source srcset="cycle-range-dark.svg" media="(prefers-color-scheme: dark)">
      <img src="cycle-range.svg" alt="A marble diagram showing the CycleRange operation">
    </picture>
</picture>

The two differ in what they do with the source. `CycleRange` accepts any `IEnumerable<T>` and must remember the
elements to replay them, so it buffers the source as it is enumerated the first time, exactly like
[`Memoize`](../enumerable-extensions/side-effects-and-evaluation.md#memoize), and therefore returns an
`IBuffer<T>` that should be disposed. `CycleMaterialized` takes a collection that is already in memory and simply
reads it again and again, with no buffer and nothing to dispose. Use `CycleMaterialized` whenever you have a
list, and `CycleRange` for a lazy or one-shot source.

Both throw an `InvalidOperationException` when the source turns out to be empty, because an empty cycle
can neither end nor produce anything. For `CycleRange` this happens on enumeration, once the source has been
read to its end.

```cs
var colors = Sequence.Return("red", "green", "blue");

var striped = rows.Zip(Sequence.CycleMaterialized(colors), (row, color) => (row, color));

using var schedule = Sequence.CycleRange(ReadShiftsFromFile());
var nextTen = schedule.Take(10).ToList();
```

## RepeatRange and RepeatMaterialized

Repeat a whole sequence a fixed number of times.

```cs
IBuffer<TSource> RepeatRange<TSource>(IEnumerable<TSource> source, int count)
IEnumerable<TSource> RepeatMaterialized<TSource>(IReadOnlyCollection<TSource> source, int count)
```

These are the finite versions of `CycleRange` and `CycleMaterialized`, with the same division of labour:
`RepeatRange` buffers a lazy source and returns an `IBuffer<T>`, `RepeatMaterialized` reads a collection
repeatedly. Unlike the cycle variants, an empty source is not an error: repeating nothing *n* times is nothing.
A count of zero yields an empty sequence.

```cs
Sequence.RepeatMaterialized(Sequence.Return(1, 2), 3);
// [1, 2, 1, 2, 1, 2]

var border = Sequence.RepeatMaterialized(Sequence.Return('-'), width).ConcatToString();
```

## Concat

Concatenates any number of sequences.

```cs
IEnumerable<TSource> Concat<TSource>(params IEnumerable<TSource>[] sources)
IEnumerable<TSource> Concat<TSource>(IEnumerable<IEnumerable<TSource>> sources)
```

<picture>
    <picture>
      <source srcset="concat-dark.svg" media="(prefers-color-scheme: dark)">
      <img src="concat.svg" alt="A marble diagram showing the Concat operation">
    </picture>
</picture>

LINQ's `Concat` takes exactly two sequences, so joining three reads `a.Concat(b).Concat(c)`. `Sequence.Concat(a, b, c)`
says the same thing once. It is lazy, enumerates the inputs in order, and is the same operation as
[`Flatten`](../enumerable-extensions/combining-sequences.md#flatten) on the sequence of sequences.

```cs
var all = Sequence.Concat(header, body, footer);
```

## Asynchronous counterparts

`Funcky.Async` provides `AsyncSequence.Concat`, `AsyncSequence.Cycle` and `AsyncSequence.CycleRange` for
`IAsyncEnumerable<T>`.
