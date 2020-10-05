# `IEnumerable` extension method `.None`
With the `.None` extension method, you can make `!enumerable.Any()` calls easier.

That's all there is. You can replace:
```csharp
if (!enumerable.Any()) { ... }
```

with the easier to read

```csharp
if (enumerable.None()) { ... }
```

Just like with `.Any()`, you can additionally pass a predicate as a parameter:

```csharp
if (enumerable.None(item => item.SomeNumericProperty == 2) { ... }
```