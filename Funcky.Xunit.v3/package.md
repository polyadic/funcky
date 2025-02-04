## About
Provides expressive assertions for xUnit.net 3.x that unpack [Funcky]'s monads.

It also enables `Option`, `Either` and `Unit` for use in theory data by providing
custom [serializers].

## Main Types
* `Funcky.FunctionalAssert`—Assertions for Funcky's monads.

## How to Use

### Test an Option
```csharp
var some = Option.Some("hello world");
var none = Option<int>.None;

// Asserts that an Option contains a value, returns its value.
var text = FunctionalAssert.Some(some);
Assert.Equal("hello world", text);

// Asserts that an Option contains the given value.
FunctionalAssert.Some("hello world", some);

// Asserts that the Option is empty.
FunctionalAssert.None(none);
```

### Test an Either
```csharp
var right = Either<string>.Return(42);
var left = Either<string, int>.Left("failure");

// Asserts that the Either is a right, returns the value.
var value = FunctionalAssert.Right(right);
Assert.Equal(42, value);

// Asserts that the Either is a right with the given value.
FunctionalAssert.Right(42, right);

// Asserts that the Either is a left, returns the value.
var message = FunctionalAssert.Left(left);
Assert.Equal("failure", message);

// Asserts that the Either is a left with the given value.
FunctionalAssert.Left("failure", left);
```

## Use an Option as theory data

```csharp
[Theory]
[MemberData(nameof(OptionValues))]
public void Example(Option<int> option) { /* ... */ }

public static TheoryData<int> OptionValues()
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
