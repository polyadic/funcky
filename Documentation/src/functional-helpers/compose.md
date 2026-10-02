# Compose

Chains two functions into one that applies the second first and then the first.

```cs
Func<TInput, TOutput> Compose<TInput, TIntermediate, TOutput>(this Func<TIntermediate, TOutput> f, Func<TInput, TIntermediate> g)
Func<TOutput> Compose<TIntermediate, TOutput>(this Func<TIntermediate, TOutput> f, Func<TIntermediate> g)
Action<TInput> Compose<TInput, TIntermediate>(this Action<TIntermediate> f, Func<TInput, TIntermediate> g)
Action Compose<TIntermediate>(this Action<TIntermediate> f, Func<TIntermediate> g)
```

`Compose` is an extension method on the *outer* function, and `f.Compose(g)` is `x => f(g(x))`, the same
order as the mathematical *f ∘ g*: `g` runs first. Read it as "f after g".

Composition lets you build a pipeline of transformations as a value, before there is any data to run it on:

```cs
Func<string, string> trim = s => s.Trim();
Func<string, string> toLower = s => s.ToLowerInvariant();
Func<string, Option<int>> parse = ParseExtensions.ParseInt32OrNone;

var parseLoosely = parse.Compose(toLower).Compose(trim);   // trim, then lower, then parse

Sequence.Return(" 42 ", "x").WhereSelect(parseLoosely);     // [42]
```

Within a LINQ query you rarely need `Compose`, because `Select(g).Select(f)` composes for you. It is for the
places where a function is passed around as a value: callbacks, strategies, and the selectors of other
higher-order functions.
