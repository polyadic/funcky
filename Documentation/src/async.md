# Funcky.Async and IAsyncEnumerable

Everything in this book so far was synchronous. This chapter covers the asynchronous side of Funcky: the
`IAsyncEnumerable<T>` counterparts of the [IEnumerable extensions](./enumerable-extensions/enumerable-extensions.md),
the `AsyncSequence` constructors, `RetryAsync`, and what it means to `await` an `Option<Task<T>>`.

## Where the APIs live

The asynchronous APIs have two homes, and which one you use depends only on your target framework.

* **.NET 10 and newer:** they are part of the `Funcky` package itself. `IAsyncEnumerable<T>` LINQ (`System.Linq.AsyncEnumerable`)
  is in the base class library since .NET 10, so Funcky can build on it without an extra dependency. You need no second
  package.
* **Older frameworks** (`netstandard2.0` up to .NET 9): they are in the `Funcky.Async` package, which compiles the same
  sources against [System.Linq.Async](https://www.nuget.org/packages/System.Linq.Async) (versions 5 and 6).

The `net10.0` and `net11.0` assets of `Funcky.Async` contain nothing but type forwards to `Funcky`. A project that referenced
`Funcky.Async` on .NET 8 and then retargets to .NET 10 therefore keeps the package reference working, and can drop
it whenever convenient. The forwards do not make the two sides source compatible, though: where the base class
library already provides an operator, Funcky defers to it, and two of them differ from the `Funcky.Async` shape.

* `AdjacentGroupBy` returns `IAsyncEnumerable<IAsyncGrouping<TKey, T>>` in `Funcky.Async`, since System.Linq.Async
  has no synchronous groupings, and `IAsyncEnumerable<IGrouping<TKey, T>>` on .NET 10. Code that treats a group as a
  sequence, `await grouping.CountAsync()` for example, becomes `grouping.Count()` after retargeting.
* `Chunk(size)` without a result selector is the base class library's on .NET 10 and yields `T[]` instead of
  `IReadOnlyList<T>`. The overloads with a result selector are Funcky's on both sides and keep `IReadOnlyList<T>`.

Namespaces and type names are otherwise the same on both sides: the extension
methods are in `Funcky.Extensions.AsyncEnumerableExtensions`, the constructors in `Funcky.AsyncSequence`, retrying in
`Funcky.AsyncFunctional`, and the `Option` awaiters in `Funcky.Monads.OptionAsyncExtensions`. With `FunckyImplicitUsings`
on, the first three namespaces are imported for you; `AsyncFunctional` is not imported statically, so `RetryAsync` is
called as `AsyncFunctional.RetryAsync` unless you add `using static Funcky.AsyncFunctional;`.

## The mental model: the .NET 10 vocabulary

`System.Linq.AsyncEnumerable` has one rule for operators that take a callback: **there is one name per operator**, and
the asynchronous form of the callback receives the cancellation token of the enumeration as its last parameter.
`Select` takes either a `Func<T, TResult>` or a `Func<T, CancellationToken, ValueTask<TResult>>`; there is no
`SelectAwait`. Funcky follows the same rule, on .NET 10 and in `Funcky.Async` alike, since 3.8.0:

```cs
IAsyncEnumerable<Order> orders = …;

// Synchronous delegate: same as on IEnumerable<T>.
var logged = orders.Inspect(order => logger.LogInformation("{Id}", order.Id));

// Asynchronous delegate: (item, cancellationToken) => ValueTask.
var audited = orders.Inspect(async (order, cancellationToken) => await audit.RecordAsync(order, cancellationToken));

// You do not need the token? Discard it.
var saved = orders.Inspect(async (order, _) => await repository.SaveAsync(order));
```

The token the delegate receives is the one passed to `GetAsyncEnumerator`, typically through
`await foreach (var x in xs.WithCancellation(token))`, or the `cancellationToken` argument of a terminal operator such
as `FirstOrNoneAsync(predicate, cancellationToken)`. Passing it on to the asynchronous call inside the delegate is what
makes cancelling the enumeration also cancel the work in progress.

Always write the two-parameter form for an asynchronous delegate, even when you discard the token. The one-parameter
`async order => …` has no asynchronous overload to bind to once the obsolete `…Await` methods are gone, and C# would
happily convert it to the `Action<T>` overload as an `async void` lambda. The enumeration then continues without
awaiting the callback, and an exception thrown by it does not fail the enumeration: it is posted to the captured
synchronization context or, without one, surfaces as an unhandled exception that can take the process down.

## The operator families

Most operators marked ⁂ in the [IEnumerable Extensions](./enumerable-extensions/enumerable-extensions.md) table exist
on `IAsyncEnumerable<T>` with the same name and the same behaviour. Operators that return a sequence keep their name
(`Pairwise`, `Merge`, `WithIndex`, …); operators that return a single value get the `Async` suffix and return a
`ValueTask` (`ElementAtOrNoneAsync`, `AggregateOrNoneAsync`, `MaterializeAsync`, …). The operators below are the ones that take
a delegate and therefore come in a synchronous and an asynchronous form.

### Lazy operators with an asynchronous delegate

These return an `IAsyncEnumerable<T>` and do nothing until enumerated. The asynchronous delegate is the
synchronous one plus a `CancellationToken` parameter, returning a `ValueTask`.

| Operator          | Synchronous delegate                     | Asynchronous delegate                                                |
|-------------------|------------------------------------------|----------------------------------------------------------------------|
| `AdjacentGroupBy` | `Func<T, TKey>` key selector (plus optional element and result selectors) | `Func<T, CancellationToken, ValueTask<TKey>>`, same for the element and result selectors |
| `Chunk`           | `Func<IReadOnlyList<T>, TResult>` result selector | `Func<IReadOnlyList<T>, CancellationToken, ValueTask<TResult>>`      |
| `ExclusiveScan`   | `Func<TAccumulate, T, TAccumulate>`      | `Func<TAccumulate, T, CancellationToken, ValueTask<TAccumulate>>`    |
| `InclusiveScan`   | `Func<TAccumulate, T, TAccumulate>`      | `Func<TAccumulate, T, CancellationToken, ValueTask<TAccumulate>>`    |
| `Inspect`         | `Action<T>`                              | `Func<T, CancellationToken, ValueTask>`                              |
| `WhereSelect`     | `Func<T, Option<TResult>>`, or with an index `Func<T, int, Option<TResult>>` | `Func<T, CancellationToken, ValueTask<Option<TResult>>>`, or `Func<T, int, CancellationToken, ValueTask<Option<TResult>>>` |

```cs
// Look up each id, skip the ones that are not found, with one name for the operator.
IAsyncEnumerable<Customer> customers = ids
    .WhereSelect(async (id, cancellationToken) => await customerStore.FindOrNoneAsync(id, cancellationToken));

// Running total with an asynchronous exchange rate lookup.
IAsyncEnumerable<decimal> runningTotal = payments
    .InclusiveScan(0m, async (total, payment, cancellationToken) => total + await ToChf(payment, cancellationToken));
```

For `AdjacentGroupBy` and `Chunk` note what the asynchronous delegate applies to: the key or result selector. The
grouping and chunking itself is the same as in the synchronous operator.

### Terminal aggregations with an asynchronous delegate

These enumerate the whole input (or as much as they need) and return a `ValueTask`. All of them take a trailing
`CancellationToken cancellationToken = default`, and the asynchronous delegate receives that same token.

| Operator            | Returns                                    | Asynchronous delegate                                                      |
|---------------------|--------------------------------------------|----------------------------------------------------------------------------|
| `AverageOrNoneAsync`| `ValueTask<Option<double>>` (or `float`/`decimal`) | selector `Func<T, CancellationToken, ValueTask<int>>` and the other numeric types, also returning `Option<…>` |
| `FirstOrNoneAsync`  | `ValueTask<Option<T>>`                     | predicate `Func<T, CancellationToken, ValueTask<bool>>`                    |
| `LastOrNoneAsync`   | `ValueTask<Option<T>>`                     | predicate `Func<T, CancellationToken, ValueTask<bool>>`                    |
| `SingleOrNoneAsync` | `ValueTask<Option<T>>`                     | predicate `Func<T, CancellationToken, ValueTask<bool>>`                    |
| `MaxOrNoneAsync`    | `ValueTask<Option<TResult>>`               | selector `Func<T, CancellationToken, ValueTask<TResult>>` or `…ValueTask<Option<TResult>>>` |
| `MinOrNoneAsync`    | `ValueTask<Option<TResult>>`               | selector `Func<T, CancellationToken, ValueTask<TResult>>` or `…ValueTask<Option<TResult>>>` |
| `NoneAsync`         | `ValueTask<bool>`                          | predicate `Func<T, CancellationToken, ValueTask<bool>>`                    |
| `PartitionAsync`    | `ValueTask<Partitions<T>>` or `ValueTask<TResult>` | predicate `Func<T, CancellationToken, ValueTask<bool>>`, optional result selector `Func<IReadOnlyList<T>, IReadOnlyList<T>, CancellationToken, ValueTask<TResult>>` |

```cs
Option<Order> firstOpen = await orders.FirstOrNoneAsync(
    async (order, cancellationToken) => await IsOpenAsync(order, cancellationToken),
    cancellationToken);

var (large, small) = await orders.PartitionAsync(async (order, _) => await IsLargeAsync(order));
```

`PartitionAsync` also has the `Either`- and `Result`-based forms of [Partition](./enumerable-extensions/filtering-and-partitioning.md#partition).
Their delegates, the `Either` selector and the result selectors, are synchronous only, so these overloads never had
an `…Await` form and were not affected by the renaming.

## Migrating from the `…Await` names

Before 3.8.0 Funcky used the vocabulary of System.Linq.Async, which has three names per operator: `Inspect` for a
synchronous delegate, `InspectAwait` for an asynchronous one, and `InspectAwaitWithCancellation` for an asynchronous one
that receives the token. The terminal aggregations followed with `FirstOrNoneAwaitAsync` and
`FirstOrNoneAwaitWithCancellationAsync`. Since 3.8.0 these overloads are `[Obsolete]`, they stay a warning (CS0618)
for the whole 3.x line, and they are removed in Funcky 4.0.0 ([#981](https://github.com/polyadic/funcky/issues/981)).

The mapping is mechanical:

| Obsolete                               | Replacement                       | Change to the delegate                      |
|----------------------------------------|-----------------------------------|---------------------------------------------|
| `xs.InspectAwaitWithCancellation(f)`   | `xs.Inspect(f)`                   | none, the delegate already takes the token  |
| `xs.InspectAwait(x => …)`              | `xs.Inspect((x, _) => …)`         | add a discarded token parameter             |
| `xs.FirstOrNoneAwaitWithCancellationAsync(f)` | `xs.FirstOrNoneAsync(f)`   | none                                        |
| `xs.FirstOrNoneAwaitAsync(x => …)`     | `xs.FirstOrNoneAsync((x, _) => …)`| add a discarded token parameter             |

The same applies to the other operators of the two families above. The obsolete message on each method spells out
its own replacement.

### Apply the code fix solution-wide

Do not do this by hand. [Funcky.Analyzers](./analyzer-rules/analyzer-rules.md) 2.1.0 ships a code fix for CS0618 on
these methods, with fix-all support. In the IDE, open the lightbulb on any of the warnings, choose the fix, and apply it
to the **solution** (the fix-all scope selector offers document, project and solution). One pass migrates every call
site. The fix:

* renames `…AwaitWithCancellation` and `…AwaitWithCancellationAsync` calls and leaves the delegate alone;
* renames `…Await` and `…AwaitAsync` calls and appends the discarded token parameter to every lambda argument,
  `x => …` becomes `(x, _) => …`;
* turns a method group or a delegate value into a forwarding lambda, because a `Func<T, ValueTask>` cannot be passed
  where a `Func<T, CancellationToken, ValueTask>` is expected: `xs.InspectAwait(Log)` becomes `xs.Inspect((x, _) => Log(x))`,
  and `xs.InspectAwait(this.inspector)` becomes `xs.Inspect((x, _) => this.inspector(x))`.

A call whose delegate argument is produced by an expression that would change meaning when evaluated per element, for
example `xs.InspectAwait(CreateInspector())`, gets no fix; the forwarding lambda would call `CreateInspector()` for
every item. Rewrite those few by hand, by storing the delegate in a local first. Anonymous methods in the
`delegate (x) { … }` syntax are not rewritten either; convert them to lambdas.

Note that `RetryAwaitAsync` in `AsyncFunctional` is **not** part of this deprecation. It is a different method that
happens to share the `Await` infix, and it stays.

## AsyncSequence

`AsyncSequence` is the asynchronous twin of the [Sequence constructors](./sequence/sequence.md). It produces an
`IAsyncEnumerable<T>` where `Sequence` produces an `IEnumerable<T>`.

| Constructor                                                      | Description                                                          |
|------------------------------------------------------------------|----------------------------------------------------------------------|
| `Return(element)`, `Return(params elements)`                     | A sequence of the given elements                                     |
| `FromNullable(element)`                                          | One element, or empty when the reference or `Nullable<T>` is `null`  |
| `Successors(first, successor)`                                   | Each element computed from the previous one by an asynchronous `Func<T, ValueTask<T>>`; endless, or finite with a `Func<T, ValueTask<Option<T>>>` successor, which stops at the first `None`. `first` may itself be an `Option<T>`. |
| `Cycle(element)`                                                 | The same element forever                                             |
| `CycleRange(source)`                                             | The elements of an `IAsyncEnumerable<T>` repeated forever            |
| `RepeatRange(source, count)`                                     | The elements of an `IAsyncEnumerable<T>` repeated `count` times      |
| `Concat(params sources)`, `Concat(IAsyncEnumerable<IAsyncEnumerable<T>>)`, `Concat(IEnumerable<IAsyncEnumerable<T>>)`, `Concat(IAsyncEnumerable<IEnumerable<T>>)` | One sequence after the other |

```cs
// Follow a paged API until it reports no next page.
IAsyncEnumerable<Page> pages = AsyncSequence.Successors(
    firstPage,
    async page => await client.NextPageOrNoneAsync(page.ContinuationToken));
```

`CycleRange` and `RepeatRange` return an `IAsyncBuffer<T>`, which is an `IAsyncEnumerable<T>` that is also
`IAsyncDisposable`: the source is enumerated once and buffered, and disposing the buffer disposes the source. The
same interface is returned by the asynchronous [Memoize](./enumerable-extensions/side-effects-and-evaluation.md#memoize). Use `await using`.

The `successor` delegate of `Successors` does not receive a cancellation token; the generator checks the token of the
enumeration between two elements instead.

## RetryAsync

[Retry](./functional-helpers/retry.md) blocks the thread with `Thread.Sleep` between attempts. `AsyncFunctional`
offers the same two families awaiting `Task.Delay` instead, with a `CancellationToken` that is checked before every
attempt and passed to `Task.Delay`, so a cancelled token stops the retrying even when the policy would allow more.

The `Option`-based family, where `None` means "not yet":

```cs
ValueTask<TResult> RetryAsync<TResult>(Func<ValueTask<Option<TResult>>> producer, CancellationToken cancellationToken = default)
ValueTask<TResult> RetryAsync<TResult>(Func<CancellationToken, ValueTask<Option<TResult>>> producer, CancellationToken cancellationToken = default)
ValueTask<Option<TResult>> RetryAsync<TResult>(Func<ValueTask<Option<TResult>>> producer, IRetryPolicy retryPolicy, CancellationToken cancellationToken = default)
ValueTask<Option<TResult>> RetryAsync<TResult>(Func<CancellationToken, ValueTask<Option<TResult>>> producer, IRetryPolicy retryPolicy, CancellationToken cancellationToken = default)
```

The exception-based family, where `shouldRetry` decides which exceptions are worth another attempt. The synchronous
producers are named `RetryAsync`, the asynchronous ones `RetryAwaitAsync`:

```cs
ValueTask<TResult> RetryAsync<TResult>(Func<TResult> producer, Func<Exception, bool> shouldRetry, IRetryPolicy retryPolicy, CancellationToken cancellationToken = default)
ValueTask RetryAsync(Action action, Func<Exception, bool> shouldRetry, IRetryPolicy retryPolicy, CancellationToken cancellationToken = default)
ValueTask<TResult> RetryAwaitAsync<TResult>(Func<ValueTask<TResult>> producer, Func<Exception, bool> shouldRetry, IRetryPolicy retryPolicy, CancellationToken cancellationToken = default)
ValueTask<TResult> RetryAwaitAsync<TResult>(Func<CancellationToken, ValueTask<TResult>> producer, Func<Exception, bool> shouldRetry, IRetryPolicy retryPolicy, CancellationToken cancellationToken = default)
ValueTask RetryAwaitAsync(Func<ValueTask> action, Func<Exception, bool> shouldRetry, IRetryPolicy retryPolicy, CancellationToken cancellationToken = default)
ValueTask RetryAwaitAsync(Func<CancellationToken, ValueTask> action, Func<Exception, bool> shouldRetry, IRetryPolicy retryPolicy, CancellationToken cancellationToken = default)
```

Prefer the overloads whose producer receives the token: they can abort an attempt that is already in progress, not
only refrain from starting the next one.

```cs
Option<Response> response = await AsyncFunctional.RetryAsync(
    async token => await TrySendAsync(request, token),             // Func<CancellationToken, ValueTask<Option<Response>>>
    new ExponentialBackOffRetryPolicy(maxRetries: 5, firstDelay: TimeSpan.FromMilliseconds(200)),
    cancellationToken);
```

The retry policies are the same `IRetryPolicy` implementations as for the synchronous `Retry`, see
[Retry policies](./functional-helpers/retry.md#retry-policies).

## Awaiting an Option of a Task

An `Option<Task<T>>` is a common shape: an operation that may or may not have been started.

```cs
Option<Task<User>> pendingUser = cache.GetValueOrNone(id).Select(entry => entry.LoadAsync());
```

Getting an `Option<User>` out of it the long way means matching on the option, awaiting in the `Some` branch and
wrapping the result again. Funcky lets you `await` the option directly:

```cs
Option<User> user = await pendingUser;
```

`None` completes immediately with `None`; `Some(task)` awaits the task and wraps its result in `Some`. The awaiters exist
for `Option<Task>`, `Option<Task<T>>`, `Option<ValueTask>` and `Option<ValueTask<T>>`; the non-generic ones complete
with no value. `ConfigureAwait` is supported as well, so library code can write

```cs
Option<User> user = await pendingUser.ConfigureAwait(false);
```

The same conversion is available as a method, for use in a query or when the result is passed on without awaiting:
`option.Sequence()` turns an `Option<Task<T>>` into a `Task<Option<T>>`, and `option.Traverse(item => LoadAsync(item))`
does the `Select` and `Sequence` in one step. Both exist for `Task`, `ValueTask` and `IAsyncEnumerable`, and the
`Traverse`/`Sequence` pairs for `Result` and `Either` are in `ResultAsyncExtensions` and `EitherAsyncExtensions`.
Finally, `option.ToAsyncEnumerable()` yields the value as a one-element `IAsyncEnumerable<T>`, or nothing.
