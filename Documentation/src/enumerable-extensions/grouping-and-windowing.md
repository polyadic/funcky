# Grouping and windowing

These operations cut one sequence into several smaller ones. They differ in whether the pieces overlap
(`SlidingWindow`, `Pairwise`), have a fixed size (`Chunk`) or are delimited by the data itself
(`AdjacentGroupBy`, `Split`).

{{#include ./chunk.md}}

{{#include ./sliding-window.md}}

{{#include ./pairwise.md}}

{{#include ./adjacent-group-by.md}}

{{#include ./split.md}}

{{#include ./take-every.md}}

{{#include ./transpose.md}}

{{#include ./power-set.md}}
