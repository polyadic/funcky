# `IEnumerable` extension method `.ForEach`
With the `.ForEach` extension method, you can invoke an action for each item in an enumerable, just like a `foreach` statement would allow you to do.

This method is already available in .NET, but just on `List`s, and it makes sense for it to be available on every enumerable.

Keep in mind that `.ForEach` is imperative and only expects an `Action<T>`. It should not be used to change state of anything outside of the `.ForEach`.
If you want to combine the enumerable into a result, consider using `.Aggregate()`, as that is designed for such use-cases.

Example:

```csharp
// Original
foreach (var item in Items)
{
   DoSomething(item);
}

// Using `.ForEach`
Items.ForEach(DoSomething); // equivalent to Items.ForEach(item => DoSomething(item));
```
