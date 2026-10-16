# Functional Programming

Functional programming is a style, not a language feature. C# has had everything it needs for the style since
lambdas and LINQ arrived, and most C# developers already write functional code every day without calling it that.
This chapter names the handful of ideas behind the style and shows where each one surfaces in Funcky. It is
deliberately short on theory. Every section ends by pointing at the part of the library that puts the idea to work.

## Pure functions and no side effects

A function is *pure* when its result depends only on its arguments, and calling it changes nothing outside of it.
It does not write to a field, a database, the console, or the clock. `Math.Max` is pure. `Console.WriteLine` is not,
and neither is a method that reads `DateTime.Now`.

Pure functions are easy to reason about because they can be understood in isolation. You can call them in any
order, call them twice, or not at all, and nothing else in the program notices. That property is what makes
LINQ work: a query is only a description, and the lambdas inside it run whenever, and however often, the
enumeration decides. A side effect inside a lambda breaks that contract in ways that are hard to see:

```cs
var count = 0;
var numbered = items.Select(item => (Number: ++count, Item: item));

numbered.First();     // count is now 1
numbered.ToList();    // count continues at 2, the first element is numbered 2
```

Funcky marks its pure methods with `[Pure]` from `System.Diagnostics.Contracts`, and the whole library is written
so that evaluating an expression has no effect beyond producing its value. Where a side effect is unavoidable,
Funcky gives it a name and a place in the pipeline instead of hiding it in a lambda:
[`Inspect`, `ForEach` and `Materialize`](../enumerable-extensions/side-effects-and-evaluation.md) make the point
where something happens visible in the code.

## Referential transparency

An expression is *referentially transparent* when it can be replaced by its value without changing the program.
`2 + 3` can be replaced by `5` anywhere. `random.Next()` cannot, because the next call returns something else.

This is the property that makes refactoring safe. If every expression is referentially transparent, you can
extract any sub-expression into a variable or a method, inline any variable back, and reorder independent
statements, all without reading the surrounding code. Pure functions give you referential transparency for free,
and side effects take it away.

It is also the reason `Option<T>` has no `Value` property. A `Value` getter that throws when there is nothing
inside is not a function of its argument alone; it depends on a check that happened, or did not happen, somewhere
else. The [Option chapter](../option.md) explains what to use instead.

## Follow the types

In a functional style the signature of a function tells you what it does, and the compiler holds you to it.
Compare these two declarations:

```cs
Customer FindCustomer(string id);
Option<Customer> FindCustomer(string id);
```

The first one might return `null`, or throw, or return an empty placeholder. You have to read the body, or the
documentation, or get bitten. The second one says in the signature that there might be no customer, and the
compiler will not let you use the result as a `Customer` until you have said what happens when there is none.

The same applies to failure. A method returning [`Result<T>`](../result.md) says that it can fail and carries the
exception with it. A method returning [`Either<L, R>`](../either.md) says it can fail *and* tells you the exact
set of ways it can. Every piece of information that was in a comment or a convention is now in the type, where
tools can see it.

Funcky's `…OrNone` methods apply this idea to the BCL. `int.Parse` throws, `int.TryParse` needs an `out` parameter,
and both hide the possibility of failure from the signature. `ParseInt32OrNone` returns an `Option<int>` and
is honest about it. The [TryVerb-pattern chapter](../try-pattern.md) lists all of them.

## Higher-order functions

A *higher-order function* takes a function as an argument or returns one. You use them every time you pass a
lambda to `Where` or `Select`. There is nothing more to the concept, but once you see functions as values, a
few things become natural that are awkward otherwise.

A function can be stored, named, and reused:

```cs
Func<Order, bool> isOpen = order => order.Status == OrderStatus.Open;

var open = orders.Where(isOpen);
var anyOpen = orders.Any(isOpen);
```

A function can be adapted. [`Flip`](../functional-helpers/flip.md) swaps the arguments of a two-argument function,
and [`Curry`](../functional-helpers/curry.md) turns a function of several arguments into a chain of functions of
one argument each, so that some arguments can be supplied now and the rest later.

And a function can be wrapped in behaviour. [`Retry`](../functional-helpers/retry.md) takes an operation and a
policy and returns the result of running the operation until it succeeds. The operation does not know it is being
retried; the retry logic does not know what it is retrying.

## Composition

Small functions are easy to write and test. Composition is how they become a program. `f.Compose(g)` builds a new
function that runs `g` and then `f`, without running either yet:

```cs
Func<string, string> trim = s => s.Trim();
Func<string, Option<int>> parse = ParseExtensions.ParseInt32OrNone;

var parseTrimmed = parse.Compose(trim);
```

Composition is the reason the pieces of Funcky are small. A `Where`, a `Select` and a `FirstOrNone` are each
trivial on their own, and a pipeline of them is a non-trivial program that needed no new code. LINQ composes
sequence operations for you, [`Compose`](../functional-helpers/compose.md) composes plain functions, and
[`All`, `Any` and `Not`](../functional-helpers/predicate-composition.md) compose predicates so that `Where`
can be given a condition built from named parts.

Composition is also what makes a pipeline readable. A chain of named steps reads top to bottom in the order
things happen. Nested calls read inside out, and a method with twelve local variables reads in whatever order the
author happened to think of them.

## Monads, briefly

The word scares people, so here is all of it. A monad is a type that wraps a value and offers two operations:
`Select`, which applies a function to the value inside and keeps the wrapper, and `SelectMany`, which applies a
function that itself returns the wrapper and flattens the two layers into one.

`IEnumerable<T>` is a monad. You have called `Select` on it for years, and `SelectMany` whenever you had a
sequence of sequences. The query syntax `from x in xs from y in f(x) select g(x, y)` is nothing but a nested
`SelectMany`.

Funcky's types follow the same shape:

| Type                           | Wraps                             | `Select` does                        |
|--------------------------------|-----------------------------------|--------------------------------------|
| `IEnumerable<T>`               | zero or more values               | apply to each element                |
| [`Option<T>`](../option.md)    | zero or one value                 | apply if there is a value            |
| [`Result<T>`](../result.md)    | a value or an exception           | apply if there is no error           |
| [`Either<L, R>`](../either.md) | a left or a right value           | apply to the right value             |
| [`Reader<E, T>`](../reader.md) | a value that needs an environment | apply once the environment is given  |
| [`Lazy<T>`](../lazy.md)        | a value computed on first use     | apply, still on first use            |

Because the shape is the same, the way of working is the same. You do not take the value out, transform it, and
put it back. You describe the transformation with `Select` or a query, and the type decides whether, when, and
how often it runs. If you are comfortable doing this with `IEnumerable<T>`, you are comfortable doing it with
`Option<T>`; the [Option chapter](../option.md) only has to show you the methods.

## Railway oriented programming

Scott Wlaschin coined the term [Railway Oriented Programming][rop] for a way of picturing a pipeline of steps that
can fail. Think of two parallel tracks. Every value travels on the *success* track. Each step is a function that
takes a value from the success track and either passes a new one on, or switches the train to the *failure* track.
Once on the failure track, the value bypasses every remaining step and arrives at the end unchanged.

```text
success ──▶ [validate] ──▶ [find customer] ──▶ [reserve stock] ──▶ [create order] ──▶ Ok(order)
                │                 │                   │
failure         └─────────────────┴───────────────────┴──────────────────────────────▶ Error(why)
```

In Funcky the two-track value is a [`Result<T>`](../result.md), or an [`Either<L, R>`](../either.md) when the
failure is not an exception. A step is any function returning one of those, and `SelectMany` is the switch that
connects steps: it runs the next step for an `Ok` and skips it for an `Error`. Query syntax hides even that:

```cs
Result<Order> PlaceOrder(OrderRequest request)
    => from valid in Validate(request)
       from customer in FindCustomer(valid.CustomerId)
       from reservation in ReserveStock(valid.ProductId, valid.Quantity)
       select CreateOrder(customer, reservation);
```

Each `from` is a step. If `FindCustomer` returns an `Error`, neither `ReserveStock` nor `CreateOrder` runs, and
`PlaceOrder` returns that error. There is no `if`, no `try`, no early `return`, and no flag variable, yet the
failure handling is complete, and the happy path reads as if failure did not exist.

The picture also explains the rest of the API. `Select` is a step that cannot fail, so it only touches the
success track. `OrElse` is a switch in the other direction, moving a failure back onto the success track with a
fallback. `Inspect` and `InspectError` are observation points on one track each. `Match` is the end of the line,
where the two tracks merge back into a plain value and the caller decides what to do with each outcome:

```cs
PlaceOrder(request).Match(
    ok: order => Results.Created($"/orders/{order.Id}", order),
    error: exception => Results.BadRequest(exception.Message));
```

The one discipline the style asks for is to stay on the tracks until the end. Unwrapping a `Result<T>` in the
middle of the pipeline, handling it, and wrapping it again is what [λ1005](../analyzer-rules/λ1005.md) through
[λ1008](../analyzer-rules/λ1008.md) warn about. The analyzers point at the operation that does the same thing
without leaving the railway.

[rop]: https://fsharpforfunandprofit.com/rop/
