# Side effects and evaluation

LINQ pipelines are lazy and, by convention, pure. Sometimes you have to step outside that: run an action for
the elements, look at what flows through the middle of a pipeline, force evaluation at a specific point, or
make sure an expensive source is evaluated only once. These operations do that explicitly, so the place where
the evaluation or the side effect happens is visible in the code rather than hidden in a `foreach`.

The distinction to keep in mind is eager versus lazy. `ForEach`, `Materialize` and `Shuffle` evaluate the input
when they are called. `Inspect`, `InspectEmpty` and `Memoize` do nothing until the result is enumerated.

{{#include ./for-each.md}}

{{#include ./inspect.md}}

{{#include ./inspect-empty.md}}

{{#include ./materialize.md}}

{{#include ./memoize.md}}

{{#include ./shuffle.md}}
