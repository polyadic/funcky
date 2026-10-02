## About
Provides async counterparts for [Funcky]'s `IEnumerable` extensions,
`IEnumerable` generators, traversal and retrying for frameworks older than .NET 10.

Starting with .NET 10, `IAsyncEnumerable` LINQ is part of the base class library and
these APIs are part of the [Funcky] package itself. The `net10.0` asset of this package
only forwards its types to `Funcky`, so existing projects keep compiling. Once you target
.NET 10 or newer, you can remove the reference to `Funcky.Async`.

## Main Types
* `Funcky.AsyncSequence`—Generate asynchronous sequences.
* `Funcky.Extensions.AsyncEnumerableExtensions`—Extensions for `IAsyncEnumerable`.
* `Funcky.AsyncFunctional`—Async Retrying.

## Feedback & Contributing
This package is released as open source under the MIT or Apache-2.0 license at your choice.
Bug reports and contributions are welcome at the [GitHub repository](https://github.com/polyadic/funcky).

[Funcky]: https://www.nuget.org/packages/Funcky
