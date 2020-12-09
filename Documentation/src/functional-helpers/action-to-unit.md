The `ActionToUnit` function wraps a action into a function that returns a `Funcky.Unit` instance.

You could write this method for yourself like this:

```csharp
public static Func<Unit> ActionToUnit(Action action) => (Func<Unit>) (() =>
{
  action();
  return new Unit(); // or default, or Funcky.Unit.Value
});
```

However, if you now wanted a wrapper for a method with a parameter going into the action, you would need to write this:

```csharp
public static Func<T, Unit> ActionToUnit<T>(Action<T> action) => (Func<T, Unit>) (parameter =>
{
  action(parameter);
  return new Unit();
});
```

Now with 2 parameters, you would need 2 generic parameters, and so on.
We already support everything from 0 up to 8 in the static class `Functional`, 
so you can just write `using static Funcky.Functional` in your using section, and start using `ActionToUnit`.

For some use cases, see the [Unit Type](./unit-type.md) documentation.

Here one example with a switch expression:

```csharp
var value = GetValue();
_ = value switch
{
	"Known" => ActionToUnit(() => Console.Write("Known")),
	_ => ActionToUnit(() => Console.Write("Unknown")),
};
```