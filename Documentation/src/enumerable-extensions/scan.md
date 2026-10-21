## InclusiveScan, ExclusiveScan

A scan is an `Aggregate` that yields every intermediate state instead of only the final one. The classic
example is the running total, also called the prefix sum.

```cs
IEnumerable<TAccumulate> InclusiveScan<TSource, TAccumulate>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> accumulator)
IEnumerable<TAccumulate> ExclusiveScan<TSource, TAccumulate>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> accumulator)
```

<picture>
    <picture>
      <source srcset="scan-dark.svg" media="(prefers-color-scheme: dark)">
      <img src="scan.svg" alt="A marble diagram showing the InclusiveScan and ExclusiveScan operations">
    </picture>
</picture>

The two variants differ in whether an element's own contribution is included in the state yielded for it:

* **`InclusiveScan`** yields the state *after* folding in each element. The last value is what `Aggregate`
  would return.
* **`ExclusiveScan`** yields the state *before* folding in each element. The first value is the seed, and the
  final accumulated state is never yielded.

Both produce exactly as many elements as the input and are lazy, which makes them usable on infinite sequences.

### Example

```cs
var amounts = Sequence.Return(10, 20, 30);

amounts.InclusiveScan(0, (sum, amount) => sum + amount);
// [10, 30, 60]

amounts.ExclusiveScan(0, (sum, amount) => sum + amount);
// [0, 10, 30]
```

The exclusive scan answers "where does each element start", for example the offset of each block in a buffer:

```cs
var offsets = blocks.ExclusiveScan(0, (offset, block) => offset + block.Length);
```

The inclusive scan answers "what is the state so far", for example an account balance after every transaction:

```cs
var balances = transactions.InclusiveScan(openingBalance, (balance, t) => balance + t.Amount);
```
