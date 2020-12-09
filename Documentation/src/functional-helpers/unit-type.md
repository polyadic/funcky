The unit type is an alternative to the native C# struct `Void` (with the global alias `void`).

The C# compiler handles `Void`/`void` very different to all other types in C#:
- `Void` can never be used. You must always use `void`
- `void` can not be instantiated
- `void` can not be used as a generic argument to any function
- A function (sometimes also called method) "returning" `void` does not return a value, and the result of the function (`void`) can not be assigned to any variable
- When using reflection, void-methods will return `null` because the compiler can not know during compile-time what the dynamically dispatched method will return

These limitations on the void type introduce annoying behaviour, especially with expressions and generics.

## Example 1 - the "void is not a real type" dilemma:

This examples uses a simple algebraic datatype with a generic match method:

```csharp
public abstract class SimpleAlgebraicDatatype
{
	private SimpleAlgebraicDatatype()
	{
	}

	public abstract TResult Match<TResult>(
		Func<Variant1, TResult> variant1,
		Func<Variant2, TResult> variant2);

	public class Variant1 : SimpleAlgebraicDatatype
	{
		public Variant1(string someValue)
		{
			SomeValue = someValue;
		}
		
		public string SomeValue { get; }

		public override TResult Match<TResult>(
			Func<Variant1, TResult> variant1, 
			Func<Variant2, TResult> variant2)
			=> variant1(this);
	}

	public class Variant2 : SimpleAlgebraicDatatype
	{
		public Variant2(int someValue)
		{
			SomeValue = someValue;
		}
		
		public int SomeValue { get; }

		public override TResult Match<TResult>(
			Func<Variant1, TResult> variant1, 
			Func<Variant2, TResult> variant2)
			=> variant2(this);
	}
}
```

It then can be used like that:

```csharp
SimpleAlgebraicDatatype variant = new SimpleAlgebraicDatatype.Variant1("Hey");
// get the variant as string
var value = variant.Match(
	variant1: variant1 => variant1.SomeValue,
	variant2: variant2 => Convert.ToString(variant2.SomeValue));
Console.WriteLine(value);
```

But if you don't want to use the return value, you're stuck with returning a value you don't want.
This does **not** compile:

```csharp
// Error [CS0411] The type arguments for method 'method' cannot be inferred from the usage. Try specifying the type arguments explicitly.
variant.Match(
	variant1: variant1 => Console.Write(variant1.SomeValue),
	variant2: variant2 => Console.Write(Convert.ToString(variant2.SomeValue)));
```

Now you have to decide what it returns. One option is to return null - the best fitting return type is probably `object?` in that case:

```csharp
variant.Match<object?>(
	variant1: variant1 =>
	{
		Console.Write(variant1.SomeValue);
		return null;
	},
	variant2: variant2 =>
	{
		Console.Write(Convert.ToString(variant2.SomeValue));
		return null;
	});
```

This is very unstatisfying however. We have to trick the type-system. There should be a more expressive way.

Funcky.Unit to the rescue:

```csharp
variant.Match(
	variant1: variant1 =>
	{
		Console.Write(variant1.SomeValue);
		return Unit.Value;
	},
	variant2: variant2 =>
	{
		Console.Write(Convert.ToString(variant2.SomeValue));
		return Unit.Value;
	});
```

Now this isn't really less noise. This is why we created [ActionToUnit](./action-to-unit.md).

This clears up the code to:

```csharp
variant.Match(
	variant1: variant1 => ActionToUnit(() => Console.Write(variant1.SomeValue)),
	variant2: variant2 => ActionToUnit(() => Console.Write(Convert.ToString(variant2.SomeValue))));
```

See [ActionToUnit](./action-to-unit.md) for an explanation.

## Example 2 - the "switch expression must return something" dilemma:

The following two code snippes do **not** comple:

```csharp
// Error [CS0201]: Only assignment, call, increment, decrement, and new object expressions can be used as a statement.
variant switch
{
	SimpleAlgebraicDatatype.Variant1 variant1 => Console.Write(variant1.SomeValue),
	SimpleAlgebraicDatatype.Variant2 variant2 => Console.Write(Convert.ToString(variant2.SomeValue)),
	_ => throw new Exception("Unreachable"),
};

// Error [CS0029]: Cannot implicitly convert type 'thorw-expression' to 'void'
// Error [CS9209]: A value of type 'void' may not be assigned.
_ = variant switch
{
	SimpleAlgebraicDatatype.Variant1 variant1 => Console.Write(variant1.SomeValue),
	SimpleAlgebraicDatatype.Variant2 variant2 => Console.Write(Convert.ToString(variant2.SomeValue)),
	_ => throw new Exception("Unreachable"),
};
```

One way to resolve this dilemma is to return a method that returns null from every arm, and execute it after:

```csharp
// very verbose and not very readable
Func<SimpleAlgebraicDatatype, object> action = variant switch
{
	SimpleAlgebraicDatatype.Variant1 variant1 => _ =>
	{
		Console.Write(Convert.ToString(variant1.SomeValue));
		return null;
	},
	SimpleAlgebraicDatatype.Variant2 variant2 => _ =>
	{
		Console.Write(Convert.ToString(variant2.SomeValue));
		return null;
	},
	_ => _ => throw new Exception("Unreachable"),
};
action(variant);
```
If we use [ActionToUnit](./action-to-unit.md) once again, we can simplify this code, by a lot:

```csharp
_ = variant switch
{
	SimpleAlgebraicDatatype.Variant1 variant1 => ActionToUnit(() => Console.Write(variant1.SomeValue)),
	SimpleAlgebraicDatatype.Variant2 variant2 => ActionToUnit(() => Console.Write(Convert.ToString(variant2.SomeValue))),
};
```

If you cannot use the discard syntax (`_ =`), simply use `var _` or similar, and ignore the variable after.