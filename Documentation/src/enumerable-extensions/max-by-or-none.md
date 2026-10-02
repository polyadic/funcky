## MaxByOrNone

Returns the element with the largest key, or `None` if the sequence is empty.

```cs
Option<TSource> MaxByOrNone<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
Option<TSource> MaxByOrNone<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
```

<picture>
    <picture>
      <source srcset="max-by-or-none-dark.svg" media="(prefers-color-scheme: dark)">
      <img src="max-by-or-none.svg" alt="A marble diagram showing the MaxByOrNone operation">
    </picture>
</picture>

`MaxByOrNone` mirrors `MinByOrNone`: it returns the element rather than the key, the first element wins on
ties, and an empty sequence yields `None` instead of throwing like the BCL's `MaxBy`.

### Example

```cs
animals.MaxByOrNone(animal => animal.Weight);     // Some(Elephant)
animals.MaxByOrNone(animal => animal.Name.Length, StringComparer.Ordinal); // Some(Elephant)
```
