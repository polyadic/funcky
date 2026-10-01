# Side effects and evaluation

LINQ pipelines are lazy and pure by convention. Sometimes you need to step outside that: log what flows through,
force evaluation at a specific point, or make sure an expensive source is only enumerated once. These operations
do that explicitly, so the place where it happens is visible in the code.

{{#include ./for-each.md}}

{{#include ./inspect.md}}

{{#include ./materialize.md}}

{{#include ./shuffle.md}}
