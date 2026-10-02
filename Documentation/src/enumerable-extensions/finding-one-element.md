# Finding one element

LINQ's `First`, `Last`, `Single`, `ElementAt`, `Min`, `Max` and `Average` all share one problem: there may be
nothing to return. The BCL answers this in two ways. The plain methods throw an `InvalidOperationException`, and
the `…OrDefault` variants return `default(T)`, which for a `List<int>` is `0`, indistinguishable from a real
element with the value `0`, and for a reference type is a `null` you have to remember to check.

Funcky adds an `…OrNone` variant for each of them that returns an `Option<T>`. Empty input is `None`, and the
value is only accessible once you have said what to do when there is none. All of them exist on `IEnumerable<T>`;
`FirstOrNone`, `LastOrNone`, `SingleOrNone` and `ElementAtOrNone` also exist on `IQueryable<T>`, where the
predicate is an expression tree and is translated by the query provider.

```cs
// Before
var first = numbers.FirstOrDefault();
if (first != 0) // wrong: 0 might be a legitimate first element
{
    ...
}

// After
numbers.FirstOrNone().Switch(
    none: () => ...,
    some: first => ...);
```

The element type is constrained to `notnull`. If your sequence contains `null`s, filter them out first with
[`WhereNotNull`](./filtering-and-partitioning.md#wherenotnull).

See the [Option Monad](../option.md) chapter for what to do with the result, and the
[TryVerb-pattern](../try-pattern.md) for the same idea applied to the rest of the BCL.

{{#include ./first-or-none.md}}

{{#include ./last-or-none.md}}

{{#include ./single-or-none.md}}

{{#include ./element-at-or-none.md}}

{{#include ./min-or-none.md}}

{{#include ./max-or-none.md}}

{{#include ./min-by-or-none.md}}

{{#include ./max-by-or-none.md}}

{{#include ./average-or-none.md}}

{{#include ./get-non-enumerated-count-or-none.md}}
