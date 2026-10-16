## Inspect

Runs an action for every element as it passes through, and yields the element unchanged.

```cs
IEnumerable<TSource> Inspect<TSource>(this IEnumerable<TSource> source, Action<TSource> inspector)
```

`Inspect` is a `Select` that returns its input, with a side effect on the way. It is **lazy**: nothing runs until
the result is enumerated, and the inspector then runs for each element at the moment that element is requested,
interleaved with everything downstream. This makes it the tool for looking into the middle of a pipeline, for
example to log, count or assert, without changing its structure.

The same name exists on `Option<T>`, `Result<T>` and `Either<L, R>` with the same meaning: observe, then hand
back unchanged.

### Example

```cs
return customers
    .Where(c => c.IsActive)
    .Inspect(c => logger.LogDebug("active customer {Id}", c.Id))
    .Select(c => c.Email);
```

### Deferred execution

Because `Inspect` is deferred, where its side effect happens depends on who enumerates the result and when:

```cs
Enumerable.Range(1, 100)
    .Inspect(n => Console.WriteLine($"before where: {n}"))
    .Where(n => n % 2 == 0)
    .Inspect(n => Console.WriteLine($"after where: {n}"))
    .Take(2)
    .ToList();   // the side effects happen here

// before where: 1
// before where: 2
// after where: 2
// before where: 3
// before where: 4
// after where: 4
```

Only four elements were inspected, because `Take(2)` stopped asking. Replace `ToList()` with nothing, and
nothing is printed at all. See the Microsoft documentation on
[deferred execution](https://learn.microsoft.com/dotnet/standard/linq/deferred-execution-lazy-evaluation) for
the general rules.
