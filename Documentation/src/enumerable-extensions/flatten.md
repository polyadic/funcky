## Flatten

Concatenates a sequence of sequences into one sequence.

```cs
IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> enumerable)
```

<picture>
    <picture>
      <source srcset="flatten-dark.svg" media="(prefers-color-scheme: dark)">
      <img src="flatten.svg" alt="A marble diagram showing the Flatten operation">
    </picture>
</picture>

`Flatten` is `SelectMany(Identity)`: the inner sequences are enumerated one after the other, in order, lazily.
It is the inverse of `Chunk`, and exists because `SelectMany(x => x)` says much less than its name.

The same operation on a known number of sequences is `Sequence.Concat(first, second, third)`, which is the
variadic form of LINQ's `Concat`.

### Example

```cs
var pages = Sequence.Return(
    Sequence.Return(1, 2, 3),
    Sequence.Return(4, 5),
    Enumerable.Empty<int>(),
    Sequence.Return(6));

pages.Flatten();
// [1, 2, 3, 4, 5, 6]

numbers.Chunk(3).Flatten().SequenceEqual(numbers);
// true
```
