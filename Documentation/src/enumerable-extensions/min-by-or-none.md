## MinByOrNone

Returns the element with the smallest key, or `None` if the sequence is empty.

```cs
Option<TSource> MinByOrNone<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
Option<TSource> MinByOrNone<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
```

<picture>
    <picture>
      <source srcset="min-by-or-none-dark.svg" media="(prefers-color-scheme: dark)">
      <img src="min-by-or-none.svg" alt="A marble diagram showing the MinByOrNone operation">
    </picture>
</picture>

Where `MinOrNone` with a selector returns the smallest *key*, `MinByOrNone` returns the *element* that has it.
This is the same distinction as between `Min(selector)` and `MinBy` in the BCL, which gained `MinBy` in .NET 6
and throws on an empty sequence of value types. `MinByOrNone` is available on every target framework and
returns `None` instead.

The first element with the smallest key wins when there are ties. The key selector is called once per
comparison, so keep it cheap or project the key first.

### Example

```cs
var animals = Sequence.Return(
    new Animal("Elephant", Weight: 6000),
    new Animal("Mouse", Weight: 0.02),
    new Animal("Cat", Weight: 4),
    new Animal("Dog", Weight: 30));

animals.MinByOrNone(animal => animal.Weight);     // Some(Mouse)
animals.MinOrNone(animal => animal.Weight);       // Some(0.02)
Enumerable.Empty<Animal>().MinByOrNone(a => a.Weight); // None
```
