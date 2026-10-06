## Memoize

Makes a sequence re-enumerable without evaluating it more than once: elements are produced lazily from the
source the first time they are requested, and served from a buffer every time after.

```cs
IBuffer<TSource> Memoize<TSource>(this IEnumerable<TSource> source)
```

<picture>
    <picture>
      <source srcset="memoize-dark.svg" media="(prefers-color-scheme: dark)">
      <img src="memoize.svg" alt="A marble diagram showing the Memoize operation">
    </picture>
</picture>

`Materialize` evaluates everything up front. `Memoize` sits between that and a plain lazy sequence: the source
is still pulled one element at a time, so an infinite or expensive source is only evaluated as far as any
consumer has looked, but every element is evaluated once, and a second enumeration replays the buffer instead of
the source. This is what you want when a sequence is expensive to produce and consumed by several operators, or
when a query is enumerated an unknown number of times.

The result is an `IBuffer<T>`, which is an `IEnumerable<T>` that is also `IDisposable`, because it owns the
enumerator of the source. Dispose it when you are done, typically with `using`; enumerating a disposed buffer
throws `ObjectDisposedException`. Memoizing a sequence that is already a list, a collection or a buffer does not
copy it.

`Memoize` is not thread-safe. Use it from a single thread, or memoize before handing the sequence out.

The `IAsyncEnumerable` variant returns an `IAsyncBuffer<T>` with the same contract, except that several consumers
may enumerate it concurrently: the source is still pulled one element at a time and every element is produced
once. The cancellation token passed to `GetAsyncEnumerator`, for example through `WithCancellation`, cancels that
consumer's enumeration only.

### Example

```cs
using var primes = Sequence.Successors(2, p => Option.Some(NextPrime(p))).Memoize();

var firstTen = primes.Take(10).ToList();
var below100 = primes.TakeWhile(p => p < 100).ToList();   // reuses the first ten, computes the rest
```

```cs
using var rows = ReadRowsFromDatabase().Memoize();

var total = rows.Sum(r => r.Amount);
var largest = rows.MaxByOrNone(r => r.Amount);   // no second database round trip
```
