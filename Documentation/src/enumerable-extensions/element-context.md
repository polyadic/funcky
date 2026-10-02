# Context for each element

A `foreach` loop often needs to know more than the element: is this the first one, the last one, what was the
one before, what is its index. The usual answer is a counter variable outside the loop. These operations pair
each element with that information instead, so it can be used in a `Select` or a query.

{{#include ./with-index.md}}

{{#include ./with-first.md}}

{{#include ./with-last.md}}

{{#include ./with-previous.md}}
