## ForEach

Runs an action for every element, immediately.

```cs
Unit ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
Unit ForEach<TSource>(this IEnumerable<TSource> source, Func<TSource, Unit> action)
```

`List<T>` has a `ForEach` method; this one works on any `IEnumerable<T>`. It is **eager**: the whole sequence is
enumerated during the call, which is the difference from [`Inspect`](#inspect). It returns
[`Unit`](../functional-helpers/unit-type.md) rather than `void`, so it can be used as the last expression of an
expression-bodied member or a lambda that must return something.

`ForEach` is the end of a pipeline, not a step in it: everything before it should be pure, and the action is where
the side effects happen. If the loop body builds up a value, that is an `Aggregate`, not a `ForEach` with a
captured variable.

### Example

```cs
// Before
foreach (var order in orders)
{
    Ship(order);
}

// After
orders.ForEach(Ship);

// As an expression-bodied member
public Unit ShipAll(IEnumerable<Order> orders)
    => orders.ForEach(Ship);
```
