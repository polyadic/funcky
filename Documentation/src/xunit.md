# Testing with xUnit

A test that receives an `Option<T>` has the same question as any other code: how do I get the value out?
In production code the answer is [`Select`, `Match` and friends](./option.md#how-do-i-get-the-value-out). In a test the
answer is an assertion, because a test *wants* to fail when the case is wrong. Funcky.Xunit provides those assertions
for `Option<T>`, `Result<T>` and `Either<L, R>`, in the same shape as xunit's own `Assert.Single` or `Assert.IsType`:
assert the case, get the content back.

## Which package

| xUnit.net version     | Package                                                             |
|-----------------------|---------------------------------------------------------------------|
| 2.x                   | [`Funcky.Xunit`](https://www.nuget.org/packages/Funcky.Xunit)       |
| v3, 1.x through 4.x   | [`Funcky.Xunit.v3`](https://www.nuget.org/packages/Funcky.Xunit.v3) |

Both packages share the assertions below and differ only in the theory-data helpers at the end of this chapter.
The assertions are static extension members of xunit's `Assert`, declared in `Funcky.Extensions.AssertExtensions`,
so they appear in the same completion list as `Assert.Equal`:

```cs
using Funcky.Extensions;

Assert.Some(42, FindUser(id).Select(user => user.Age));
```

Extension members need the C# 14 compiler, which the .NET 10 SDK that Funcky 4 requires provides. Up to Funcky.Xunit
2.x and Funcky.Xunit.v3 1.x the same assertions were also methods of a `Funcky.FunctionalAssert` class,
`FunctionalAssert.Some(42, option)`; Funcky.Xunit 3.0.0 and Funcky.Xunit.v3 2.0.0 removed that class. On the last
2.x release the analyzer rule λ1203 rewrites every call to the `Assert` form, which is the upgrade path.

### xUnit.net 3.x versions

xUnit.net v3 has had three major releases since 1.0, each breaking binary compatibility with the previous one for
extensibility authors. Funcky.Xunit.v3 is unaffected: it uses only `Assert`, `XunitException`, `ArgumentFormatter`
and the serializer interfaces, none of which changed. The package is compiled against `xunit.v3.assert` and
`xunit.v3.extensibility.core` 1.0.1 and declares the range `[1.0.1, 5)`, so NuGet takes whichever version your test
project references.

What does change between the xunit versions is the test host and the runner, not Funcky. In particular, with the
.NET 10 SDK, xunit.v3 4.x no longer runs under VSTest unless you opt in, which also affects the Azure DevOps
`DotNetCoreCLI@2` task.

| xunit.v3 | Funcky.Xunit.v3                   | Test project runs on           | `dotnet test` with the .NET 10 SDK                                                |
|----------|-----------------------------------|--------------------------------|-----------------------------------------------------------------------------------|
| 1.x      | 1.0.0 and later                   | .NET 6+, .NET Framework 4.7.2+ | VSTest (`xunit.runner.visualstudio`) or Microsoft.Testing.Platform v1             |
| 2.x      | 1.1.0 and later, expected to work | .NET 8+, .NET Framework 4.7.2+ | VSTest or Microsoft.Testing.Platform v1                                           |
| 3.x      | 1.1.0 and later, expected to work | .NET 8+, .NET Framework 4.7.2+ | VSTest or Microsoft.Testing.Platform v1                                           |
| 4.x      | 1.1.0 and later, verified         | .NET 8+, .NET Framework 4.7.2+ | Microsoft.Testing.Platform v2; VSTest only with the `xunit.v3.*.mtp-off` packages |

"Verified" means Funcky's own test suite runs green against that version. "Expected to work" means nothing in the
xunit release notes touches what Funcky uses, but the suite is not run against it yet
([#1015](https://github.com/polyadic/funcky/issues/1015) tracks a matrix build). The test host column is xunit's
minimum; Funcky.Xunit.v3 itself targets `netstandard2.0` and `net8.0`.

If `dotnet test` on the .NET 10 SDK stops with "Testing with VSTest target is no longer supported by
Microsoft.Testing.Platform", either opt into Microsoft.Testing.Platform in `global.json`
(`"test": { "runner": "Microsoft.Testing.Platform" }`) or reference the `mtp-off` variants of the xunit packages. A
v3 test project is an executable, so `dotnet run --project` works whichever runner the SDK prefers.

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
Assert.Some() Failure: Values differ
Expected: Some(10)
Actual:   None
```

```text
Assert.Ok() Failure: Values differ
Expected: Ok(10)
Actual:   Error(System.InvalidOperationException: "boom")
```

```text
Assert.Error() Failure: Values differ
Expected: Error(ArgumentException)
Actual:   Error(InvalidOperationException)
```

The returning overloads cannot know the expected content and print `Some(...)`, `Ok(...)`, `Error(...)`,
`Left(...)` or `Right(...)` as the expected side. The label matches xunit's own `Assert.Equal() Failure`; up to
Funcky.Xunit 2.x it read `FunctionalAssert.Some()`, so a test that asserts on the message text changes with 3.0.0.

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
* [λ1101](./analyzer-rules/λ1101.md) is the analyzer rule for test code.
