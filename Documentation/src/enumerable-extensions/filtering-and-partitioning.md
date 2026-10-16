# Filtering and partitioning

LINQ's `Where` keeps or drops elements by a predicate. These operations cover the cases around it: filtering and
transforming in one step when the transformation can fail (`WhereSelect`), removing `null` while fixing the type
(`WhereNotNull`), keeping both halves instead of only one (`Partition`), asking whether anything is left (`None`),
and substituting a fallback when nothing is (`AnyOrElse`).

{{#include ./where-select.md}}

{{#include ./where-not-null.md}}

{{#include ./partition.md}}

{{#include ./none.md}}

{{#include ./any-or-else.md}}
