# Apply

Partially applies a function: fixes some of its arguments and returns a function of the remaining ones.

```cs
// For every function of two to five parameters and every choice of which to fix, for example:
Func<T2, TResult> Apply<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 p1, Unit p2)
Func<T1, TResult> Apply<T1, T2, TResult>(Func<T1, T2, TResult> func, Unit p1, T2 p2)
Func<T1, T3, TResult> Apply<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, Unit p1, T2 p2, Unit p3)
// … and the same as extension methods on Func<…>
```

Pass a value for every argument you want to fix and `__` (two underscores, from `Funcky.Discard`) for every
one you want to leave open. The result takes the open parameters in their original order. Unlike
[`Curry`](./curry.md), which fixes arguments from the left one at a time, `Apply` fixes any subset in one call:

```cs
using static Funcky.Discard;

int Linear(int m, int x, int c) => (m * x) + c;

var line = Fn(Linear);
var doubled = line.Apply(2, __, 0);        // x => Linear(2, x, 0)
var offsetBy = line.Apply(__, 10, __);     // (m, c) => Linear(m, 10, c)

doubled(21);                               // 42
offsetBy(1, 1);                            // 11
```

The function form, `Apply(line, 2, __, 0)`, is the same thing; use whichever reads better. Both exist for two
to five parameters with every combination of fixed and open positions, which is why the overload list in
IntelliSense is long. As with `Curry`, a method group needs [`Fn`](./curry.md#fn) so the type arguments can
be inferred.

## How the open positions are typed

`__` is `Unit.Value`, and the overloads use `Unit` as the parameter type of an open position. That is what lets
overload resolution pick the right shape from the arguments alone. The price is that a function whose parameter
really is of type `Unit` cannot be partially applied on that parameter, because fixing it and leaving it open
look the same. That case is rare enough not to matter in practice; if you hit it, wrap the function in a lambda
that drops the `Unit` parameter first.

## When to use which

| You want to                                      | Use                        |
|--------------------------------------------------|----------------------------|
| Fix the first argument and pass the rest on      | `Curry(f)(a)`              |
| Fix the second argument of a two-argument method | `Flip(f).Curry()(b)` or `f.Apply(__, b)` |
| Fix any mix of arguments in one step             | `f.Apply(…)`               |
| Build a one-argument-at-a-time function          | `Curry`                    |

`Apply` is the most direct of the three when the function has three or more parameters, because it does not
force you to think about argument order.
