# `IEnumerable` extension method `.Inspect`
With the `.Inspect` extension method, you can invoke an action for each item in an enumerable, 
just like `.ForEach` or the `foreach` statement would allow you to do, but the method yields the initial enumerable back.


This can be useful when you want to apply a side-effect to a list before returning, or continue selecting on a list after applying a side-effect.
`Inspect` can be especially useful when you want to log step(s) of a complex query, since you don't have to change the structure of the code to use it.

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
