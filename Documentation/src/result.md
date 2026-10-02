# Result Monad

`Result<T>` represents the outcome of a computation that may fail. It is either `Ok` with a value of type `T`,
or `Error` with an `Exception`. Where [`Option<T>`](./option.md) only tells you *that* something is missing,
`Result<T>` also tells you *why*.

The type is deliberately tied to `Exception`. Funcky does not introduce its own error hierarchy; the error case
carries whatever your code or the BCL already throws. This makes `Result<T>` the natural return type for a
function that would otherwise throw, and it means an error can be turned back into a thrown exception at the
boundary without losing information. When the error type should be something else, use
[`Either<L, R>`](./either.md).

## Think of it as `Option<T>` with a reason

Every method you know from `Option<T>` exists on `Result<T>` with the same name and the same meaning. The only
difference is that the empty case is called `error` instead of `none`, and that it carries a value:

| `Option<T>`        | `Result<T>`              |
|--------------------|--------------------------|
| `Some(value)`      | `Ok(value)`              |
| `None`             | `Error(exception)`       |
| `Match(none, some)`| `Match(ok, error)`       |
| `Switch(none, some)`| `Switch(ok, error)`     |
| `InspectNone`      | `InspectError`           |
| `ToNullable`       | `GetOrThrow`             |

`Select`, `SelectMany`, `OrElse`, `GetOrElse` and `Inspect` are identical, and query syntax works the same way.
The one operation missing is `Where`: filtering out a value would need an exception to put in the `Error` case,
and there is no sensible default for it. Apart from that, the ladder in the Option chapter, from "stay inside with
`Select`" to "handle both cases with `Match`", applies without change.

## Creating results

```cs
// Ok
Result<int> ok = Result.Ok(42);
Result<int> alsoOk = 42;                    // implicit conversion from T
Result<int> viaReturn = Result.Return(42);  // same as Ok, the name monads use

// Error
Result<int> error = Result<int>.Error(new InvalidOperationException("boom"));
```

The type parameter is constrained to `notnull`, and `Ok(null)` throws. Unlike `Option<T>`, a `Result<T>` has
no meaningful `default`: the struct is marked `[NonDefaultable]`, and [λ1009](./analyzer-rules/λ1009.md) from
`Funcky.Analyzers` reports an error if you try to construct one with `default`.

`Result<T>.Error` has one side effect worth knowing about. If the exception has not been thrown yet, it has no
stack trace, and the error would be useless once it surfaces somewhere else. `Error` therefore captures the
current stack trace on the exception when none is set. The `Error(…)` call site is where the stack trace points,
which is almost always what you want.

## Chaining operations that may fail

The typical use of `Result<T>` is a pipeline of steps where each step can go wrong and the first failure
should stop the rest:

```cs
Result<Order> PlaceOrder(string customerId, string productId, int quantity)
    => from customer in FindCustomer(customerId)
       from product in FindProduct(productId)
       from stock in ReserveStock(product, quantity)
       select new Order(customer, product, quantity);
```

If `FindCustomer` returns an `Error`, neither `FindProduct` nor `ReserveStock` run, and the `Error` is passed
through unchanged with its exception intact. There is no `try` and no `catch`, and the signature of `PlaceOrder`
tells the caller that it can fail. Compare that with the exception-based version, where the only way to know
which exceptions a method throws is to read its body.

## Recovering from an error

`OrElse` and `GetOrElse` have two overloads each. The one that takes a plain fallback discards the exception,
the one that takes a function receives it, so you can decide based on what went wrong:

```cs
// Discards the exception: every failure is treated the same.
Result<Config> config = ReadConfig(path).OrElse(Result.Ok(Config.Default));

// Receives the exception: recover from some failures, keep the rest.
Result<Config> config = ReadConfig(path).OrElse(exception => exception switch
{
    FileNotFoundException => Result.Ok(Config.Default),
    _ => Result<Config>.Error(exception),
});
```

The same applies to `GetOrElse`, which leaves the result and gives you a `T`:

```cs
int retries = ReadSetting("retries").GetOrElse(3);
int retries = ReadSetting("retries").GetOrElse(exception => LogAndDefault(exception, 3));
```

## Leaving the result at the boundary

### `Match` and `Switch`

As with options, `Match` is the general tool and should be the last one you reach for:

```cs
IActionResult response = PlaceOrder(customerId, productId, quantity).Match(
    ok: order => Created(order),
    error: exception => BadRequest(exception.Message));
```

`Switch` is the same for actions. Always name the arguments; [λ1003](./analyzer-rules/λ1003.md) enforces it, and
[λ1005](./analyzer-rules/λ1005.md) to [λ1007](./analyzer-rules/λ1007.md) flag `Match` calls that should have been
`GetOrElse` or `OrElse`.

### `GetOrThrow`

`Result<T>` is often the right type inside your code and the wrong type at the edge, for example in a
`Main` method, a test, or a framework callback that expects exceptions. `GetOrThrow` returns the value in the
`Ok` case and rethrows the exception in the `Error` case:

```cs
Order order = PlaceOrder(customerId, productId, quantity).GetOrThrow();
```

The exception is rethrown with its original stack trace preserved, not wrapped and not reset. Together with the
capture in `Error`, this means a failure deep in a pipeline reports the location where it was created, even
though the `throw` happens somewhere else entirely.

### `Inspect` and `InspectError`

Both run an action and return the result unchanged, which is convenient for logging in the middle of a chain:

```cs
Result<Order> order = PlaceOrder(customerId, productId, quantity)
    .Inspect(order => logger.LogInformation("placed order {Id}", order.Id))
    .InspectError(exception => logger.LogError(exception, "order failed"));
```

## Working with many results

### `Sequence` and `Traverse`

A sequence of results is rarely what you want to hand on; usually you want one result that is `Ok` with all the
values, or the first `Error`. `Sequence` does exactly that for an `IEnumerable<Result<T>>`, and `Traverse` combines
it with a `Select`:

```cs
Result<IReadOnlyList<Order>> orders = orderRequests
    .Traverse(request => PlaceOrder(request.CustomerId, request.ProductId, request.Quantity));
```

If every call succeeds, the result is `Ok` with a list of orders. If any call fails, the result is that `Error`,
and the remaining requests are not processed.

### `Partition`

When you want to process everything and collect both the failures and the successes, `Partition` splits an
`IEnumerable<Result<T>>` into two lists:

```cs
var (errors, orders) = orderRequests
    .Select(request => PlaceOrder(request.CustomerId, request.ProductId, request.Quantity))
    .Partition();

logger.LogWarning("{Count} orders failed", errors.Count);
```

`Partition` materializes the whole sequence. It is also available with a result selector that receives both
lists as separate parameters.

### Combining with other monads

`Traverse` and `Sequence` also exist for swapping a `Result<T>` with another monad that sits inside it:
`Result<Option<T>>` becomes `Option<Result<T>>`, `Result<Either<L, R>>` becomes `Either<L, Result<R>>`, and so on
for `Lazy<T>`, `Reader<E, T>` and `IEnumerable<T>`. The same operations exist in the other direction on
`Option<T>`, so `Option<Result<T>>.Sequence()` gives you a `Result<Option<T>>`.

## Where to go next

* [Option Monad](./option.md) explains the common API in more depth; everything there applies here too.
* [Either Monad](./either.md) is the choice when the error should be something other than an `Exception`.
