## About
Provides expressive assertions for xUnit.net v3 (1.x to 4.x) that unpack [Funcky]'s monads.

It also enables `Option`, `Either` and `Unit` for use in theory data by providing
custom [serializers].

## Main Types
* `Funcky.Extensions.AssertExtensions`—Assertions for Funcky's monads as static extension members of xunit's `Assert`: `Assert.Some`, `Assert.None`, `Assert.Ok`, `Assert.Error`, `Assert.Left`, `Assert.Right`.

The assertions are C# 14 extension members, so they need the .NET 10 SDK or newer; `using Funcky.Extensions;` brings
them into scope.

## How to Use

### Test an Option
```csharp
var some = Option.Some("hello world");
var none = Option<int>.None;

// Asserts that an Option contains a value, returns its value.
var text = Assert.Some(some);
Assert.Equal("hello world", text);

// Asserts that an Option contains the given value.
Assert.Some("hello world", some);

// Asserts that the Option is empty.
Assert.None(none);
```

### Test a Result
```csharp
// Asserts that the Result is Ok, returns the value.
var value = Assert.Ok(result);

// Asserts that the Result is an error, returns the exception.
var error = Assert.Error(result);
var typed = Assert.Error<InvalidOperationException, int>(result);
```

### Test an Either
```csharp
var right = Either<string>.Return(42);
var left = Either<string, int>.Left("failure");

// Asserts that the Either is a right, returns the value.
var value = Assert.Right(right);
Assert.Equal(42, value);

// Asserts that the Either is a right with the given value.
Assert.Right(42, right);

// Asserts that the Either is a left, returns the value.
var message = Assert.Left(left);
Assert.Equal("failure", message);

// Asserts that the Either is a left with the given value.
Assert.Left("failure", left);
```

## Use an Option as theory data

```csharp
[Theory]
[MemberData(nameof(OptionValues))]
public void Example(Option<int> option) { /* ... */ }

public static TheoryData<Option<int>> OptionValues()
   => [
       Option<int>.None,
       Option.Some(10),
       Option.Some(20),
   ];
```

## Feedback & Contributing
This package is released as open source under the MIT or Apache-2.0 license at your choice.
Bug reports and contributions are welcome at the [GitHub repository].


[Funcky]: https://www.nuget.org/packages/Funcky
[serializers]: https://xunit.net/docs/getting-started/v3/custom-serialization
[GitHub repository]: https://github.com/polyadic/funcky
