# `IEnumerable` extension method `.None`
With the `.None` extension method, you can make `!enumerable.Any()` calls easier.

That's all there is. You can replace:
```csharp
if (!enumerable.Any()) 
{
  ...
}
```

with the easier to read

```csharp
if (enumerable.None())
{
  ...
}