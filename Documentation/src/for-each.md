# `IEnumerable` extension method `.ForEach`
With the `.ForEach` extension method, you can invoke an action for each item in an enumerable, just like a `for each` statement would allow you to do.
This method is already available in .NET, but just on `List`s, and it makes sense for it to be available on every enumerable.

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