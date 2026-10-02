# Flip

Swaps the first two parameters of a function.

```cs
Func<T2, T1, TResult> Flip<T1, T2, TResult>(Func<T1, T2, TResult> function)
Func<T2, T1, T3, TResult> Flip<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> function)
// … up to eight parameters, and the same for Action<…>
```

Only the first two parameters are swapped; any further ones keep their position.

`Flip` matters in combination with [`Curry`](./curry.md). Partial application fixes the *first* argument, so
if the argument you know is the second one, flip first:

```cs
// Math.Pow(x, y) is x^y. We want "square", so y is the one we know.
var pow = Fn(Math.Pow);
var square = Flip(pow).Curry()(2.0);   // x => Math.Pow(x, 2.0)

square(3.0);   // 9
```

The same applies to any two-argument method whose "configuration" parameter comes first and whose data
parameter comes second, such as `string.Join(separator, values)`:

```cs
Func<string, IEnumerable<string>, string> join = string.Join;
var commaSeparated = Flip(join).Curry()(items);   // separator => string.Join(separator, items)
```
