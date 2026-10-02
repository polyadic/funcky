# Context for each element

A `foreach` loop often needs to know more than the element itself: is this the first one, is it the last one,
what came before it, what is its index. The usual answer is a counter or a flag declared outside the loop and
updated inside it. These operations pair each element with that information instead, so it is available in a
`Select` or a query, and the loop state disappears.

The four `With…` operations return small structs with a `Value` property and the extra information, each with a
`Deconstruct` so they work in tuple patterns and `foreach`. All are lazy, and when the source is an `IList<T>`
the result is a list too, so `Count` and indexing stay cheap.

`InclusiveScan` and `ExclusiveScan` generalise the idea: the context is an arbitrary state accumulated over all
preceding elements.

{{#include ./with-index.md}}

{{#include ./with-first.md}}

{{#include ./with-last.md}}

{{#include ./with-previous.md}}

{{#include ./scan.md}}
