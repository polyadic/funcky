The `Identity` function is designed to replace parameter-returning lambdas, like sometimes used in LINQ.

Example 1:
```csharp
// Method:
public void FunctionExpectingSelector<TIn, TOut>(Func<TIn, TOut> selector)
{
  // ...
}

// Usually:
FunctionExpectingSelector(x => x);
// Or:
FunctionExpectingSelector(item => item);

// With Identity:
FunctionExpectingSelector(Identity);
```

Example 2 (typical `SelectMany` selector):
```csharp
// Usually result of a query:
IEnumerable<IEnumerable<int>> itemGroups = new[] { new[] { 1, 2, 3 }, new[] { 5, 6, 7 } };

// Goal: Get all items flattened.
// Common approach:
itemGroups.SelectMany(x => x);
// Or:
itemGroups.SelectMany(items => items);

// With Identity:
itemGroups.SelectMany(Identity);
```