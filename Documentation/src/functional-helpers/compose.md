# Compose and Then

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

Enumerable.Return(" 42 ", "x").WhereSelect(parseLoosely);     // [42]
```

Within a LINQ query you rarely need `Compose`, because `Select(g).Select(f)` composes for you. It is for the
places where a function is passed around as a value: callbacks, strategies, and the selectors of other
higher-order functions.

## Then

`Then` is the same composition read forwards: `f.Then(g)` is `x => g(f(x))`, which is `g.Compose(f)`.

```cs
Func<TInput, TOutput> Then<TInput, TIntermediate, TOutput>(this Func<TInput, TIntermediate> f, Func<TIntermediate, TOutput> next)
Func<TOutput> Then<TIntermediate, TOutput>(this Func<TIntermediate> f, Func<TIntermediate, TOutput> next)
Action<TInput> Then<TInput, TIntermediate>(this Func<TInput, TIntermediate> f, Action<TIntermediate> next)
Action Then<TIntermediate>(this Func<TIntermediate> f, Action<TIntermediate> next)
```

Use whichever reads better at the call site. `Compose` matches the mathematical notation and puts the outer
function first; `Then` lists the steps in the order they run, like a LINQ chain, which is usually what you want
for a pipeline of more than two steps:

```cs
var parseLoosely = trim.Then(toLower).Then(parse);         // the same function as above

Func<Order, Receipt> process = validate.Then(price).Then(persist);
```

The overloads taking an `Action` end the pipeline with a side effect and return an `Action`.
