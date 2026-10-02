# Either Monad

`Either<L, R>` holds a value of exactly one of two types. It is either `Left` with a value of type `L`,
or `Right` with a value of type `R`. Neither side is privileged by the type itself, but the API treats `Right`
as the success case and `Left` as the failure case, following the convention of other functional languages
(mnemonic: "right" is right).

That makes `Either<L, R>` the generalisation of [`Result<T>`](./result.md): a `Result<T>` is an
`Either<Exception, T>` with a few conveniences for exceptions. Reach for `Either<L, R>` when your failure case is
not an exception, for example a domain error type, a list of validation messages, or simply a second kind of
successful outcome.

```cs
public abstract record OrderError;
public sealed record CustomerNotFound(string CustomerId) : OrderError;
public sealed record OutOfStock(string ProductId, int Available) : OrderError;

Either<OrderError, Order> PlaceOrder(string customerId, string productId, int quantity);
```

The caller now sees from the signature not just that `PlaceOrder` can fail, but how, and the compiler can check
that every failure is handled.

## The same ladder, one more type parameter

Every operation from [`Option<T>`](./option.md) and `Result<T>` exists on `Either<L, R>`, operating on the
`Right` side. The `Left` value is passed through untouched, exactly like `None` or `Error`:

| `Result<T>`           | `Either<L, R>`         |
|-----------------------|------------------------|
| `Ok(value)`           | `Right(value)`         |
| `Error(exception)`    | `Left(value)`          |
| `Match(ok, error)`    | `Match(left, right)`   |
| `Switch(ok, error)`   | `Switch(left, right)`  |
| `InspectError`        | `InspectLeft`          |

`Select`, `SelectMany`, `OrElse`, `GetOrElse` and `Inspect` work the same way, and so does query syntax. As with
`Result<T>` there is no `Where`, because a filtered-out value would need a `Left` to replace it. The fallback overloads of `OrElse` and `GetOrElse` receive the `Left` value, so you can recover based on what went
wrong.

## Creating eithers

```cs
Either<OrderError, Order> right = Either<OrderError, Order>.Right(order);
Either<OrderError, Order> alsoRight = order;                    // implicit conversion from R
Either<OrderError, Order> viaReturn = Either<OrderError>.Return(order);

Either<OrderError, Order> left = Either<OrderError, Order>.Left(new OutOfStock(productId, available: 0));
```

Both type parameters are constrained to `notnull`. There is no implicit conversion from `L`, because an
`Either<string, string>` would be ambiguous, and because constructing the failure case explicitly is a feature.

`Either<L, R>` has no valid `default`. The struct is marked `[NonDefaultable]`, [λ1009](./analyzer-rules/λ1009.md)
from `Funcky.Analyzers` reports an error for `default(Either<…>)`, and any operation on such a value throws `NotSupportedException` at runtime.

`Either<L>.Return` looks odd at first. It is the monad's `Return` with the left type fixed and the right type
inferred, and it exists so that `Return` can be passed as a method group where the left type is already known:

```cs
Either<OrderError, Order> fallback = either.Match(left: Recover, right: Either<OrderError>.Return);
```

[λ1006](./analyzer-rules/λ1006.md) will point out that this particular example is `either.OrElse(Recover)`.

## Chaining

```cs
Either<OrderError, Order> PlaceOrder(string customerId, string productId, int quantity)
    => from customer in FindCustomer(customerId)
       from product in FindProduct(productId)
       from stock in ReserveStock(product, quantity)
       select new Order(customer, product, quantity);
```

All steps must share the same `Left` type. When they do not, `SelectLeft` maps the left side without touching the
right one, which is how you lift a step with a specific error type into a pipeline with a more general one:

```cs
Either<CustomerNotFound, Customer> LookupCustomer(string customerId);

Either<OrderError, Customer> FindCustomer(string customerId)
    => LookupCustomer(customerId).SelectLeft(notFound => (OrderError)notFound);
```

## Operations specific to `Either<L, R>`

### `SelectLeft`

`Select` transforms the right value; `SelectLeft` transforms the left value. Together they let you map both
sides, and `Select` followed by `SelectLeft` is the bifunctor map you may know from other libraries.

### `Flip`

`Flip` swaps the sides, turning an `Either<L, R>` into an `Either<R, L>`. It is occasionally useful when a
method treats the "wrong" side as the success case, and you want to continue with the LINQ operators on the
other one.

### `LeftOrNone` and `RightOrNone`

Both project an `Either<L, R>` to an `Option<T>` for one side, discarding the other:

```cs
Option<OrderError> error = PlaceOrder(customerId, productId, quantity).LeftOrNone();
Option<Order> order = PlaceOrder(customerId, productId, quantity).RightOrNone();
```

The opposite direction is `Option<R>.ToEither(left)`, which turns a `None` into a `Left` with the given value:

```cs
Either<CustomerNotFound, Customer> LookupCustomer(string customerId)
    => customers.GetValueOrNone(customerId).ToEither(new CustomerNotFound(customerId));
```

This is the usual way to attach a reason to an option coming out of an `…OrNone` method, and it has a lazy
overload for a left value that is expensive to construct. Note that the left type is inferred from the argument,
so passing a `CustomerNotFound` yields an `Either<CustomerNotFound, …>`; specify the type arguments explicitly or
use `SelectLeft` when you need the base type.

## Working with many eithers

### `Sequence` and `Traverse`

`Traverse` runs a function returning `Either<L, R>` over a sequence and stops at the first `Left`:

```cs
Either<OrderError, IReadOnlyList<Order>> orders = orderRequests
    .Traverse(request => PlaceOrder(request.CustomerId, request.ProductId, request.Quantity));
```

`Sequence` is the same for a sequence that already holds eithers. Both also exist for swapping an
`Either<L, R>` with a monad inside its right side, such as `Either<L, Option<R>>` to `Option<Either<L, R>>`.

### `Partition`

`Partition` splits a sequence of eithers into all the left and all the right values, materializing the sequence:

```cs
var (errors, orders) = orderRequests
    .Select(request => PlaceOrder(request.CustomerId, request.ProductId, request.Quantity))
    .Partition();
```

There is also an overload that takes the selector directly, so the `Select` above can be folded into the
`Partition` call, and overloads that take a result selector receiving both lists.

## Where to go next

* [Option Monad](./option.md) explains the common API in more depth.
* [Result Monad](./result.md) is the specialisation for `Exception` as the left type.
