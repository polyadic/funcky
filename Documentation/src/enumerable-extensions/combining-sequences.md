# Combining sequences

These operations take two or more sequences and produce one. They differ in how they pick the next element:
`Merge` by comparing, `Interleave` by taking turns, `ZipLongest` by position, `Flatten` by exhausting one
sequence before starting the next. `Intersperse` is the odd one out: it combines a sequence with a single
element.

All of them are lazy and enumerate each input at most once.

{{#include ./merge.md}}

{{#include ./interleave.md}}

{{#include ./intersperse.md}}

{{#include ./zip-longest.md}}

{{#include ./flatten.md}}

{{#include ./cartesian-product.md}}
