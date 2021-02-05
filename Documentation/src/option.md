# Option Monad

## What is the Option Monad

The Option Monad is a very simple [algebraic type](https://en.wikipedia.org/wiki/Algebraic_data_type) which is a fancy way to say you can have more than one different type of data in it. The Option monad is a combination of a value of a type and a second type which can only be one value: `None`. The state of the option monad is always either None, or Some with a certain value of a type you can chose. It means it can hold any value of your chosen type + None state.

This is very similar to references which can be `null` or [Nullable Value Types](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/nullable-value-types) which adds the "has no value"-concept to value types.

However the main issue with references and `Nullable<T>` is, before every access you need to check if the value is accessible. The Option Monad is an abstraction which removes all this boilerplate code, in a save way.

### Create something

```csharp
var something = Option.Some(1337);
```

### Create nothing

```csharp
var nothing = Option<int>.None();
```

### Select

```csharp
Option<bool> maybeBool =
    from m in maybe
    select m == 1337;
```

### Select Many

```csharp
var result = from number in someNumber
    from date in someDate
    select Tuple.Create(number, date);
```

### Match

```csharp
bool isSome = maybe.Match(
    none: false,
    some: m => true
);
```

## How can I get the value?

If you declare

```cs
int? integer = 1337;
```

You can access the Value directly via `i.Value`. The typical beginner question on the monad is therefore how to get to the value in a monad.

The Option-Monad intentionally has no way to get to the value directly because that would be an unsafe operation. The whole point of an optional is that it sometimes has no value. Instead you should inject the behaviour into the monad.

The basic Example:

```cs
int? integer = MaybeValue();

if (integer.HasValue())
{
    Console.WriteLine($"Value: {integer.Value}");
}
```

Injecting the behaviour:

```cs
Option<int> integer = MaybeValue();

integer
  .AndThen(i => Console.WriteLine($"Value: {i}"));
```

Or in Linq syntax:

```cs
from integer in MaybeValue()
select Console.WriteLine($"Value: {integer.Value}");
```
