# Curry and Uncurry

`Curry` turns a function of several parameters into a chain of functions taking one parameter each.
`Uncurry` is the inverse.

```cs
Func<T1, Func<T2, TResult>> Curry<T1, T2, TResult>(Func<T1, T2, TResult> function)
Func<T1, Func<T2, Func<T3, TResult>>> Curry<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> function)
// … up to eight parameters, and the same for Action<…>, where the innermost function is an Action

Func<T1, T2, TResult> Uncurry<T1, T2, TResult>(Func<T1, Func<T2, TResult>> function)
// … likewise
```

A curried function can be applied to its arguments one at a time, and applying it to the first argument gives
you a new function that remembers it. That is partial application, and it is the practical reason to curry in
C#: you fix the arguments you know and pass the rest on as a smaller function.

```cs
Func<int, int, int> add = (a, b) => a + b;
var curriedAdd = Curry(add);       // Func<int, Func<int, int>>

var addFive = curriedAdd(5);       // Func<int, int>
addFive(3);                        // 8

Enumerable.Range(1, 3).Select(curriedAdd(10));   // [11, 12, 13]
```

The curried form is what you need when an API wants a `Func<T, TResult>` and you have a two-argument method
plus one of the arguments at hand. Without `Curry` that is a lambda, `x => Add(10, x)`, which is fine once
and noisy when repeated.

`Uncurry` goes the other way, which is rarely needed in C# but completes the pair: `Uncurry(Curry(f))` is `f`.

## Fn

Method groups do not have a type of their own, so the compiler cannot infer the type parameters of `Curry`
from one:

```cs
var pow = Curry(Math.Pow);
// error CS0411: The type arguments for method 'Functional.Curry<...>' cannot be inferred from the usage.
```

`Fn` is `Identity` restricted to the job of giving a method group its natural delegate type. Wrapping the method
group makes the inference work:

```cs
var pow = Curry(Fn(Math.Pow));
// or, using the extension method form
var pow = Fn(Math.Pow).Curry();

var square = pow(2.0);   // wait, that is 2^x
```

That last line shows why [`Flip`](./flip.md) exists.
