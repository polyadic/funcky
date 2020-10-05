# `IEnumerable` extension method `.Inspect`
With the `.Inspect` extension method, you can invoke an action for each item in an enumerable, 
just like `.ForEach` or the `for each` statement would allow you to do, but the method yields the initial enumerable back.

This can be useful when you want to apply a side-effect to a list before returning, or continue selecting on a list after applying a side-effect.

Example 1:

```csharp
// Original using .ForEach
var items = someList.Select(TransformSomething);
Items.ForEach(DoSomething);
return items;

// Using `.Inspect`
return someList.Select(TransformSomething).Inspect(DoSomething);
```

Example 2:

```csharp
// Original using foreach
var items = someList.Select(TransformToSomething);
foreach (var item in items) 
{
  DoSomething(item);
}
var transformedItems = items.Select(TransformToSomethingElse);

// Using `.Inspect`
var transformedItems = someList
  .Select(TransformSomething)
  .Inspect(DoSomething)
  .Select(TransformToSomethingElse);
```