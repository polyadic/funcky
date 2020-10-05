# `IEnumerable` extension method `.Inspect`
With the `.Inspect` extension method, you can invoke an action for each item in an enumerable, 
just like `.ForEach` or the `for each` statement would allow you to do, but the method yields the initial enumerable back.

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

## Deferred Execution

It is important to understand at which moment `.Inspect` is executed. The exact moment of execution is the same as if it were a `Select`, `Where` or any other deferred LINQ-method.
See [Microsoft Docs](https://docs.microsoft.com/en-us/dotnet/standard/linq/deferred-execution-lazy-evaluation) for more information about deferred execution in LINQ.
This is also an important difference between `.ForEach` (eager) and `.Inspect` (deferred).

Consider the following example:

```csharp
Enumerable.Range(1, 100)
  .Inspect(n => Console.WriteLine($"before where: {n}"))
  .Where(n => n % 2 == 0)
  .Inspect(n => Console.WriteLine($"after where: {n}"))
  .Inspect(Console.WriteLine)
  .Take(2)
  .ToImmutableList(); // <- Side effects of .Inspect happen here
  
// Prints:
// before where: 1
// before where: 2
// after where: 2
// 2
// before where: 3
// before where: 4
// after where: 4
// 4
```