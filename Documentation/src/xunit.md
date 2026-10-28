# Testing with xUnit

A test that receives an `Option<T>` has the same question as any other code: how do I get the value out?
In production code the answer is [`Select`, `Match` and friends](./option.md#how-do-i-get-the-value-out). In a test the
answer is an assertion, because a test *wants* to fail when the case is wrong. Funcky.Xunit provides those assertions
for `Option<T>`, `Result<T>` and `Either<L, R>`, in the same shape as xunit's own `Assert.Single` or `Assert.IsType`:
assert the case, get the content back.

## Which package

| xUnit.net version | Package                                                               |
|-------------------|-----------------------------------------------------------------------|
| 2.x               | [`Funcky.Xunit`](https://www.nuget.org/packages/Funcky.Xunit)         |
| 3.x               | [`Funcky.Xunit.v3`](https://www.nuget.org/packages/Funcky.Xunit.v3)   |

Both packages share the assertions below and differ only in the theory-data helpers at the end of this chapter.
The assertions are static extension members of xunit's `Assert`, declared in `Funcky.Extensions.AssertExtensions`,
so they appear in the same completion list as `Assert.Equal`:

```cs
using Funcky.Extensions;

Assert.Some(42, FindUser(id).Select(user => user.Age));
```

Extension members need the C# 14 compiler (.NET 10 SDK). Older compilers use the same assertions as methods of
the `Funcky.FunctionalAssert` class, `FunctionalAssert.Some(42, option)`. The two forms are the same code; the
`Assert` members are the implementation and `FunctionalAssert` forwards to them. If both are available,
[λ1203](./analyzer-rules/λ1203.md) asks for the `Assert` form and comes with a code fix.

## The assertions

Each monad gets one assertion per case. The assertion for the case that carries a value comes in two overloads:
one that takes the expected value and compares it, and one that returns the value so you can continue asserting on it.

| Monad            | Case      | Expected-value overload                   | Returning overload                                    |
|------------------|-----------|-------------------------------------------|-------------------------------------------------------|
| `Option<T>`      | `Some`    | `Assert.Some(expected, option)`           | `T Assert.Some(option)`                               |
| `Option<T>`      | `None`    | `Assert.None(option)`                     |                                                       |
| `Result<T>`      | `Ok`      | `Assert.Ok(expected, result)`             | `T Assert.Ok(result)`                                 |
| `Result<T>`      | `Error`   |                                           | `Exception Assert.Error(result)`                      |
| `Result<T>`      | `Error`   |                                           | `TException Assert.Error<TException, T>(result)`      |
| `Either<L, R>`   | `Left`    | `Assert.Left(expected, either)`           | `L Assert.Left(either)`                               |
| `Either<L, R>`   | `Right`   | `Assert.Right(expected, either)`          | `R Assert.Right(either)`                              |

```cs
[Fact]
public void ParsesTheAnswer()
    => Assert.Some(42, "42".ParseInt32OrNone());

[Fact]
public void RejectsText()
    => Assert.None("forty-two".ParseInt32OrNone());

[Fact]
public void PlacesTheOrder()
{
    // The returning overload: unwrap, then assert on the content.
    var order = Assert.Ok(PlaceOrder(customerId, productId, quantity: 3));
    Assert.Equal(3, order.Quantity);
    Assert.NotEqual(default, order.Id);
}

[Fact]
public void RejectsUnknownCustomers()
{
    // Error<TException, T> asserts the exact exception type and returns it typed,
    // the analogue of Assert.Throws<T>.
    var exception = Assert.Error<CustomerNotFoundException, Order>(PlaceOrder("nobody", productId, quantity: 3));
    Assert.Equal("nobody", exception.CustomerId);
}

[Fact]
public void ValidatesTheInput()
    => Assert.Left("quantity must be positive", Validate(quantity: 0));
```

`Assert.Error(result)` without type arguments accepts any exception and returns it as `Exception`.
`Assert.Error<TException, T>(result)` requires the exception to be of *exactly* `TException`, not a subtype, which is
the same rule `Assert.Throws<T>` applies to thrown exceptions. The second type argument is the `T` of the
`Result<T>`; C# cannot infer it when only one type argument is given, so both are spelled out.

The expected value is compared with `Assert.Equal`, so the same equality xunit uses everywhere applies, including
its structural comparison of collections.

### Prefer the expected-value overload

When the test only checks for equality, do not unwrap and compare in two steps:

```cs
// Two assertions, two places to read.
Assert.Equal(42, Assert.Some(option));

// One assertion, and a failure message that shows the whole option.
Assert.Some(42, option);
```

[λ1101](./analyzer-rules/λ1101.md) flags the first form and rewrites it to the second. The single call is not only
shorter, it also fails better. Both forms report `None` as an assertion failure, but only the single call knows what
was expected and prints `Expected: Some(42)` where the two-step form can only say `Some(...)`. And when the option is
`Some(41)`, the two-step form fails in `Assert.Equal` with `41` against `42` and no trace of the monad, while the single
call reports `Expected: Some(42)` and `Actual: Some(41)`.

## How a failure reads

A failed assertion throws an `XunitException`, so the test runner shows it like any other failed assertion. The
message names the assertion and prints both sides in the notation the monads use in `ToString`:

```text
FunctionalAssert.Some() Failure: Values differ
Expected: Some(10)
Actual:   None
```

```text
FunctionalAssert.Ok() Failure: Values differ
Expected: Ok(10)
Actual:   Error(System.InvalidOperationException: "boom")
```

```text
FunctionalAssert.Error() Failure: Values differ
Expected: Error(ArgumentException)
Actual:   Error(InvalidOperationException)
```

The returning overloads cannot know the expected content and print `Some(...)`, `Ok(...)`, `Error(...)`,
`Left(...)` or `Right(...)` as the expected side. The label says `FunctionalAssert` in both forms, so a message
looks the same whichever receiver the test used.

On .NET 6 and later the assertion's own frames are hidden from the stack trace with `[StackTraceHidden]`, so the
first line of the trace is the test that failed. On older frameworks a rethrow erases the internal frames of the
monad and the lambdas, but the assertion method itself remains the first frame, with the test right below it.

## Why there is no `Assert.True(option.IsSome)`

The [introduction](./introduction.md#the-one-idea-behind-funcky) explains that Funcky's monads have no `IsSome`, no
`HasValue` and no `Value`. Tests are the place where that is most tempting to miss, because a test legitimately wants
to look inside. These assertions are the answer: they *are* the test-side way to look inside, and they are better
than a boolean check would be.

`Assert.True(option.IsSome)` would tell you that the option was empty and nothing else. `Assert.Some(42, option)`
tells you that it was `None` when `Some(42)` was expected, or that it was `Some(41)`. And because the returning
overload hands you the content, there is never a reason to read it a second time through a `Value` property; the
assertion already proved the case, and the type system carries that proof to the next line.

## Theory data

### `ToTheoryData` (xUnit.net 2.x)

xunit 2.x theories take their rows from a `TheoryData<…>`, which is filled with repeated `Add` calls. Funcky.Xunit
adds `ToTheoryData` on any `IEnumerable<T>`, and on sequences of `Tuple` and `ValueTuple` with up to ten components,
so the rows can come from a LINQ query:

```cs
[Theory]
[MemberData(nameof(SquaresUpToTen))]
public void SquaresArePositive(int number, int square)
    => Assert.True(square >= number);

public static TheoryData<int, int> SquaresUpToTen()
    => Enumerable.Range(0, 10)
        .Select(number => (number, number * number))
        .ToTheoryData();
```

Each tuple becomes one row, each component one parameter. The extension lives in `Funcky.Extensions` and is not
part of Funcky.Xunit.v3: xunit 3.x `TheoryData` accepts a collection expression, `TheoryData<int> rows = [1, 2, 3];`,
which covers the same need.

### `Option`, `Either` and `Unit` as theory data (xUnit.net 3.x)

xunit 3.x runs a theory with any data, but only rows it can serialize are discovered individually: each shows up
as its own test in the runner and can be rerun on its own. A row with a value xunit cannot serialize collapses the
whole theory into a single test case. Funcky.Xunit.v3 ships serializers for `Option<T>`, `Either<L, R>` and `Unit`,
so these types work as theory parameters without losing per-row discovery:

```cs
[Theory]
[MemberData(nameof(Options))]
public void SelectingTheIdentityChangesNothing(Option<int> option)
    => Assert.Equal(option, option.Select(value => value));

public static TheoryData<Option<int>> Options()
    => [Option<int>.None, Option.Some(10), Option.Some(20)];
```

An `Option<T>` or `Either<L, R>` is serializable when its content is serializable by xunit; `None` always is.

The serializers register themselves. The package's build targets add the three assembly attributes
`[assembly: Funcky.Xunit.RegisterOptionSerializer]`, `[assembly: Funcky.Xunit.RegisterEitherSerializer]` and
`[assembly: Funcky.Xunit.RegisterUnitSerializer]` to every project that has `IsTestProject` set to `true`, which
`Microsoft.NET.Test.Sdk` does. For a project that does not, set the MSBuild property `FunckyRegisterXunitSerializers` to
`true`; set it to `false` to opt out and add the attributes by hand where you need them.

## Where to go next

* [Option Monad](./option.md), [Result Monad](./result.md) and [Either Monad](./either.md) describe the types the
  assertions unwrap.
* [λ1101](./analyzer-rules/λ1101.md) and [λ1203](./analyzer-rules/λ1203.md) are the two analyzer rules for test code.
