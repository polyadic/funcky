## About
Provides expressive assertions for xUnit.net 2.x that unpack [Funcky]'s monads.

## Main Types
* `Funcky.FunctionalAssert`—Assertions for Funcky's monads, also available as `Assert.Some`, `Assert.Ok`, … on xunit's `Assert` (C# 14).
* `Funcky.Extensions.ToTheoryDataExtension`—Conveniently convert `IEnumerable`s to `TheoryData<..>`.

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

### Use the assertions on `Assert`
With the C# 14 compiler the same assertions are static extension members of xunit's `Assert`:
```csharp
var text = Assert.Some(some);
Assert.Some("hello world", some);
Assert.None(none);
var value = Assert.Ok(result);
var error = Assert.Error(result);
var left = Assert.Left(either);
var right = Assert.Right(either);
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

### Create theory data from an `IEnumerable`

```csharp
public static TheoryData<string, int> Integers()
    => Enumerable.Range(0, 10)
        .Select(n => (n.ToString(), n))
        .ToTheoryData();
```

## Feedback & Contributing
This package is released as open source under the MIT or Apache-2.0 license at your choice.
Bug reports and contributions are welcome at the [GitHub repository].

[Funcky]: https://www.nuget.org/packages/Funcky
[GitHub repository]: https://github.com/polyadic/funcky
