# Strings and monads

Two small groups that end a pipeline in a type other than `IEnumerable<T>`.

`JoinToString` and `ConcatToString` turn a sequence into a string; they exist because `string.Join` and
`string.Concat` are static methods that read inside out at the end of a query.

`Sequence` and `Traverse` turn a sequence of monadic values into a monadic value of a sequence. They are how
the [Option](../option.md), [Result](../result.md) and [Either](../either.md) monads scale from one value to
many: every element must succeed, or the whole thing fails with the first failure.

{{#include ./join-to-string.md}}

{{#include ./concat-to-string.md}}

{{#include ./sequence.md}}

{{#include ./traverse.md}}
