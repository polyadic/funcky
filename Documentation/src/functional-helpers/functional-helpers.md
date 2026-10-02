# Functional Helpers

The static class `Funcky.Functional` collects small, general-purpose functions that make higher-order code
read better. It is designed to be used with a static import, and Funcky's implicit usings add
`using static Funcky.Functional;` for you, so all of them are called without a prefix. The examples in this
chapter assume that.

Most of them exist to replace a lambda that says nothing with a name that says something: `x => x` becomes
`Identity`, `_ => {}` becomes `NoOperation`, `_ => true` becomes `True`. The rest are the classic
function combinators, `Curry`, `Flip` and `Compose`, and a `Retry` helper for operations that may fail
transiently.

| Helper                                          | Purpose                                                        |
|-------------------------------------------------|----------------------------------------------------------------|
| [Identity](./identity.md)                       | Returns its argument; the selector that selects nothing        |
| [NoOperation](./no-operation.md)                | Does nothing; the action for callbacks you don't need          |
| [True, False](./predicate-composition.md#true-and-false) | Constant predicates                                   |
| [All, Any, Not](./predicate-composition.md)     | Combine predicates into a new predicate                        |
| [Curry, Uncurry](./curry.md)                    | Convert between multi-argument and one-argument-at-a-time functions |
| [Flip](./flip.md)                               | Swap the first two parameters of a function                    |
| [Compose](./compose.md)                         | Chain two functions into one                                   |
| [Fn](./curry.md#fn)                             | Give a method group a delegate type so it can be passed on     |
| [Retry](./retry.md)                             | Repeat an operation until it succeeds, with a delay policy     |
| [Unit](./unit-type.md)                          | The type with one value, for "returns nothing" in generics     |
| [ActionToUnit, UnitToAction](./action-to-unit.md) | Convert between `Action` and `Func<…, Unit>`                 |

`Curry`, `Uncurry`, `Flip` and `Compose` also exist as extension methods on `Func<…>` and `Action<…>`, in
`Funcky.Extensions`, so they can be chained: `Fn(Math.Pow).Curry()`.
