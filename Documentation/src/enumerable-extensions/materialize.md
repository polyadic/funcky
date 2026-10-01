## Materialize

Evaluates a lazy sequence into a collection, unless it already is one.

```cs
IReadOnlyCollection<TSource> Materialize<TSource>(this IEnumerable<TSource> source)
IReadOnlyCollection<TSource> Materialize<TSource, TMaterialization>(this IEnumerable<TSource> source, Func<IEnumerable<TSource>, TMaterialization> materializer)
    where TMaterialization : IReadOnlyCollection<TSource>
```

A method that receives an `IEnumerable<T>` and needs to enumerate it twice, or needs its `Count`, usually calls
`ToList()`. That copies the input even when the caller already passed a list. `Materialize` only does work
when it has to: an `IReadOnlyCollection<T>` is returned as is, an `IList<T>` or `ICollection<T>` is wrapped
without copying, and only a genuinely lazy sequence is evaluated, by default into an `ImmutableList<T>`.

The second overload lets you choose the collection for the lazy case, for example `Materialize(Enumerable.ToArray)`
or `Materialize(Enumerable.ToHashSet)`. Note that it is only used when the input is not already a collection.

The result is `IReadOnlyCollection<T>`, which is the honest type: you can count it and enumerate it any number
of times, and the caller's list is not copied, so it is not necessarily yours to keep.

### Example

```cs
void Report(IEnumerable<Order> orders)
{
    var materialized = orders.Materialize();

    Console.WriteLine($"{materialized.Count} orders");
    foreach (var order in materialized) { ... }   // no second evaluation of a lazy query
}
```
